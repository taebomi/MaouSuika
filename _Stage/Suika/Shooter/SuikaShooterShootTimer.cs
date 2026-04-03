using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace SOSG.Stage.Suika.Shooter
{
    public class SuikaShooterShootTimer : MonoBehaviour
    {
        private readonly int[] _effectHashArr =
        {
            Animator.StringToHash("ShootTimer_None"),
            Animator.StringToHash("ShootTimer_1_Enter"),
            Animator.StringToHash("ShootTimer_2_Enter"),
            Animator.StringToHash("ShootTimer_3_Enter"),
        };

        [SerializeField] private SuikaShooter shooter;
        [SerializeField] private SuikaShooterAim aim;

        [SerializeField] private Animator ani;
        [SerializeField] private TMP_Text tmp;

        private float _shootTimeLimit = 5f;
        private float _remainedTime;
        private bool _isRunning;

        private int _lastTriggeredSec;

        private CancellationTokenSource _timerCts;


        private void OnEnable()
        {
            shooter.Shot += OnShot;
        }

        private void OnDisable()
        {
            shooter.Shot -= OnShot;
            StopTimer();
        }

        private void OnDestroy()
        {
            _timerCts?.Dispose();
        }

        public void SetUp(float timeLimit)
        {
            _shootTimeLimit = timeLimit;
        }

        public void StartTimer()
        {
            if (_isRunning)
            {
                return;
            }

            _isRunning = true;
            _timerCts?.Dispose();
            _timerCts = new CancellationTokenSource();
            Timer(_timerCts.Token).Forget();
        }

        public void StopTimer()
        {
            if (_isRunning is false)
            {
                return;
            }

            _isRunning = false;
            _timerCts.Cancel();
        }

        private void OnShot()
        {
            ResetTimer();
        }

        private async UniTask Timer(CancellationToken ct)
        {
            _remainedTime = _shootTimeLimit;
            _lastTriggeredSec = 4;

            while (_isRunning && ct.IsCancellationRequested is false)
            {
                if (shooter.CanShoot is false)
                {
                    await UniTask.Yield(ct);
                    continue;
                }

                _remainedTime -= Time.deltaTime;
                CheckTime(_remainedTime);

                if (_remainedTime <= 0f)
                {
                    ForceShoot();
                    ResetTimer();
                }

                await UniTask.Yield(ct);
            }
        }

        private void ResetTimer()
        {
            _remainedTime = _shootTimeLimit;
            _lastTriggeredSec = 4;
            ani.Play(_effectHashArr[0]);
        }

        private void ForceShoot()
        {
            shooter.ShootSuika(aim.LastDir, aim.LastRatio);
        }

        private void CheckTime(float remainedTime)
        {
            var curSec = Mathf.RoundToInt(remainedTime);
            if (curSec < _lastTriggeredSec && curSec > 0)
            {
                _lastTriggeredSec = curSec;
                tmp.text = $"{curSec}";
                if (curSec < _effectHashArr.Length)
                {
                    ani.Play(_effectHashArr[curSec]);
                }
            }
        }
    }
}