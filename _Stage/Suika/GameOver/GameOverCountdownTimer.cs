using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using SOSG.Stage.Suika.Shooter;
using UnityEngine;

namespace SOSG.Stage.Suika.GameOver
{
    /// <summary>
    /// 비활성화, 활성화 => enabled 이용
    /// </summary>
    public class GameOverCountdownTimer : MonoBehaviour
    {
        private const float CountdownDuration = 5f;

        [SerializeField] private SuikaShooter shooter;

        public bool IsCountdown { get; private set; }

        private CancellationTokenSource _countdownCts;

        public event Action CountdownStarted;
        public event Action CountdownCanceled;
        public event Action CountdownFinished;

        private void OnEnable()
        {
            SetUpCallbacks();
            if (shooter.CurState is SuikaShooter.State.Collided)
            {
                StartCountdown();
            }
        }

        private void OnDisable()
        {
            TearDownCallbacks();
            StopCountdown();
        }

        private void SetUpCallbacks()
        {
            shooter.StateChanged += OnShooterStateChanged;
        }

        private void TearDownCallbacks()
        {
            shooter.StateChanged -= OnShooterStateChanged;
        }

        private void OnShooterStateChanged(SuikaShooter.State state)
        {
            if (state is SuikaShooter.State.Collided)
            {
                StartCountdown();
            }
            else
            {
                StopCountdown();
            }
        }

        private void StartCountdown()
        {
            if (IsCountdown)
            {
                return;
            }

            IsCountdown = true;
            _countdownCts?.Dispose();
            _countdownCts = new CancellationTokenSource();
            Countdown(_countdownCts.Token).Forget();
        }


        private void StopCountdown()
        {
            if (IsCountdown is false)
            {
                return;
            }

            IsCountdown = false;
            _countdownCts.Cancel();
            CountdownCanceled?.Invoke();
        }

        private async UniTaskVoid Countdown(CancellationToken ct)
        {
            CountdownStarted?.Invoke();

            var timer = CountdownDuration;
            while (timer > 0f && ct.IsCancellationRequested is false)
            {
                var collidedSuika = shooter.CollideChecker.CollidedSuika;
                if (collidedSuika is not null && IsMoving(collidedSuika))
                {
                    timer = CountdownDuration;
                }
                else
                {
                    timer -= Time.deltaTime;
                }

                await UniTask.Yield();
            }

            IsCountdown = false;
            CountdownFinished?.Invoke();
        }

        private static bool IsMoving(SuikaObject suika)
        {
            const float velocityThreshold = 0.25f;
            const float sqrVelocityThreshold = velocityThreshold * velocityThreshold;

            var suikaVelocitySqrMag = suika.PhysicsComponent.Rb.linearVelocity.sqrMagnitude;
            return suikaVelocitySqrMag > sqrVelocityThreshold;
        }
    }
}