using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace SOSG.Stage.Suika.Shooter
{
    public class SuikaShooterCollideChecker : MonoBehaviour
    {
        [SerializeField] private SuikaShooter shooter;

        [SerializeField] private SuikaCollection suikaCollection;

        public SuikaObject CollidedSuika { get; private set; }
        public bool IsCollided { get; private set; }


        private CancellationTokenSource _checkingCts;

        private void OnEnable()
        {
            _checkingCts?.Dispose();
            _checkingCts = new CancellationTokenSource();
            CheckCollided(_checkingCts.Token).Forget();
        }

        private void OnDisable()
        {
            _checkingCts.Cancel();
        }

        private async UniTaskVoid CheckCollided(CancellationToken ct)
        {
            while (ct.IsCancellationRequested is false)
            {
                var loadedSuika = shooter.LoadedSuika;
                if (loadedSuika is null)
                {
                    await UniTask.Yield(ct);
                    continue;
                }

                var collidedColl =
                    Physics2D.OverlapCircle(transform.position, loadedSuika.Radius, PhysicsCache.SuikaLayerMask);
                var isCollidedNow = (bool)collidedColl;
                if (IsCollided != isCollidedNow) // 충돌 상태가 변경되었을 경우
                {
                    CollidedSuika = isCollidedNow ? suikaCollection.GetSuika(collidedColl.GetInstanceID()) : null;
                    IsCollided = isCollidedNow;
                    if (isCollidedNow)
                    {
                        shooter.OnCollidedEnter();
                    }
                    else
                    {
                        shooter.OnCollidedExit();
                    }
                }

                await UniTask.Yield(ct);
            }
        }
    }
}