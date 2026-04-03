using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace SOSG.Area
{
    public abstract class BattleArea : MonoBehaviour
    {
        public AreaData AreaData { get; private set; }

        protected FloatVarSO OverlordMoveXSpeedVarSO;

        protected CancellationTokenSource DestroyCts;

        public const float AreaWidth = 48f;

        private void Awake()
        {
            DestroyCts = new CancellationTokenSource();
        }

        protected virtual void AwakeAfter()
        {
        }

        private void OnDestroy()
        {
            DestroyCts.CancelAndDispose();
        }

        public void Initialize(AreaData areaData, FloatVarSO overlordMoveXSpeedVarSO)
        {
            AreaData = areaData;
            OverlordMoveXSpeedVarSO = overlordMoveXSpeedVarSO;
            InitializeAfter(areaData);
        }

        protected virtual void InitializeAfter(AreaData areaData)
        {
        }


        public void Destroy()
        {
            Destroy(gameObject);
        }

        public abstract void InitializePosition(Vector3 localPos);

        public abstract void OnMainSceneEvent(int timing);

        public abstract UniTask Appear();
        public abstract UniTask Disappear();
        public abstract UniTaskVoid ScrollArea();
    }
}