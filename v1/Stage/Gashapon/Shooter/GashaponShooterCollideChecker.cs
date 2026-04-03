using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace SOSG.Stage
{
    public class GashaponShooterCollideChecker : MonoBehaviour
    {
        [Header("Event SO")]
        [SerializeField] private VoidEventSO stageStartEventSO;
        [SerializeField] private VoidEventSO stageEndEventSO;

        [Header("Variable SO")]
        [SerializeField] private GashaponVarSO curLoadedGashaponVarSO;
        [SerializeField] private GashaponVarSO curCollidedGashaponVarSO;
        [SerializeField] private BoolVariableSO isCollidedVarSO;
        [SerializeField] private GashaponColliderDictVarSO gashaponColliderDictVarSO;

        private int _gashaponLayerMask;

        private CancellationTokenSource _checkingCts;

        private void Awake()
        {
            isCollidedVarSO.Initialize(false);
            curCollidedGashaponVarSO.value = null;

            _gashaponLayerMask = PhysicsCache.GetLayerMask(PhysicsCache.LayerName.Gashapon);
            
            stageStartEventSO.OnEventRaised += OnStageStarted;
            stageEndEventSO.OnEventRaised += OnStageEnded;
        }

        private void OnDestroy()
        {
            stageStartEventSO.OnEventRaised -= OnStageStarted;
            stageEndEventSO.OnEventRaised -= OnStageEnded;
            
            _checkingCts?.CancelAndDispose();
        }


        private void OnStageStarted()
        {
            _checkingCts?.CancelAndDispose();
            _checkingCts = new CancellationTokenSource();

            CheckShooterCollided(_checkingCts.Token).Forget();
        }

        private void OnStageEnded()
        {
            _checkingCts?.Cancel();
        }

        private async UniTaskVoid CheckShooterCollided(CancellationToken ct)
        {
            while (_checkingCts.IsCancellationRequested is false)
            {
                var curLoadedGashapon = curLoadedGashaponVarSO.value;
                if (!curLoadedGashapon)
                {
                    await UniTask.Yield(ct);
                    continue;
                }

                var collidedColl = GetCollidedColliderWithGashapon(curLoadedGashapon);
                var isCollided = (bool)collidedColl;
                if (isCollidedVarSO.Value != isCollided)
                {
                    curCollidedGashaponVarSO.value = isCollided
                        ? gashaponColliderDictVarSO[collidedColl.GetInstanceID()]
                        : null;
                    isCollidedVarSO.Set(isCollided);
                }

                await UniTask.Yield(ct);
            }
        }

        private Collider2D GetCollidedColliderWithGashapon(Gashapon target) =>
            Physics2D.OverlapCircle(transform.position, target.CurSize * 0.5f, _gashaponLayerMask);
    }
}