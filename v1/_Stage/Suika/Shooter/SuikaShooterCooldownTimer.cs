using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using SOSG.Stage.Suika.Shooter;
using UnityEngine;

namespace SOSG.Stage.Suika
{
    public class SuikaShooterCooldownTimer : MonoBehaviour
    {
        private const double CooldownDuration = 0.5;

        [SerializeField] private SuikaShooter shooter;

        public bool IsCooldown { get; private set; }

        private CancellationTokenSource _cooldownCts;

        private void OnEnable()
        {
            _cooldownCts?.Dispose();
            _cooldownCts = new CancellationTokenSource();
            IsCooldown = false;
        }

        private void OnDisable()
        {
            _cooldownCts.Cancel();
            IsCooldown = false;
        }

        public void StartCooldown()
        {
            if (IsCooldown)
            {
                _cooldownCts.CancelAndDispose();
                _cooldownCts = new CancellationTokenSource();
            }

            Cooldown().Forget();
        }

        private async UniTaskVoid Cooldown()
        {
            IsCooldown = true;
            shooter.OnCooldownEnter();
            await UniTask.Delay(TimeSpan.FromSeconds(CooldownDuration), cancellationToken: _cooldownCts.Token);
            IsCooldown = false;
            shooter.OnCooldownExit();
        }
    }
}