using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace SOSG.Stage.GameOver
{
    public class WarningSystem : MonoBehaviour
    {
        [Header("Event SO")]
        [SerializeField] private VoidEventSO stageStartEventSO;
        [SerializeField] private GameOverSystemStateSO gameOverSystemStateSO;

        [Header("Variable SO")]
        [SerializeField] private GashaponColliderDictVarSO gashaponCollDict;

        [SerializeField] private BoolVariableSO isWarningVarSO;

        [Header("Setting")]
        [SerializeField] private Vector2 minAreaPos;
        [SerializeField] private Vector2 maxAreaPos;

        private Collider2D[] _gashaponCollArr;
        private ContactFilter2D _gashaponFilter;

        private int _warningCount;

        private CancellationTokenSource _checkingCts;

        private const float CheckInterval = 0.5f;
        private const int WarningCountThreshold = 3;

        private void Awake()
        {
            _checkingCts = new CancellationTokenSource();
            _gashaponCollArr = new Collider2D[3];
            _gashaponFilter = new ContactFilter2D
            {
                layerMask = PhysicsCache.GetLayerMask(PhysicsCache.LayerName.Gashapon),
                useLayerMask = true
            };

            _warningCount = 0;

            stageStartEventSO.OnEventRaised += OnStageStarted;
            gameOverSystemStateSO.ActionOnGameOver += OnGameOver;
        }

        private void OnDestroy()
        {
            _checkingCts?.CancelAndDispose();
            stageStartEventSO.OnEventRaised -= OnStageStarted;
            gameOverSystemStateSO.ActionOnGameOver -= OnGameOver;
        }
        

        private void OnStageStarted()
        {
            Check(_checkingCts.Token).Forget();
        }

        private void OnGameOver()
        {
            _checkingCts?.Cancel();
        }


        private async UniTaskVoid Check(CancellationToken ct)
        {
            while (ct.IsCancellationRequested is false)
            {
                if (!IsSafe())
                {
                    AddWarning();
                }
                else
                {
                    SubWarning();
                }

                await UniTask.Delay(TimeSpan.FromSeconds(CheckInterval), cancellationToken: ct);
            }
        }

        private void AddWarning()
        {
            if (_warningCount >= WarningCountThreshold)
            {
                if (!isWarningVarSO.Value)
                {
                    isWarningVarSO.Set(true);
                }
            }
            else
            {
                _warningCount++;
            }
        }

        private void SubWarning()
        {
            if (_warningCount <= 0)
            {
                if (isWarningVarSO.Value)
                {
                    isWarningVarSO.Set(false);
                }
            }
            else
            {
                _warningCount--;
            }
        }

        private bool IsSafe()
        {
            var count = Physics2D.OverlapArea(minAreaPos, maxAreaPos, _gashaponFilter, _gashaponCollArr);
            for (var i = 0; i < count; i++)
            {
                var gashaponCollID = _gashaponCollArr[i].GetInstanceID();
                var gashapon = gashaponCollDict[gashaponCollID];
                if (gashapon.IsGrounded)
                {
                    return false;
                }
            }

            return true;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube((minAreaPos + maxAreaPos) * 0.5f, maxAreaPos - minAreaPos);
        }
#endif
    }
}