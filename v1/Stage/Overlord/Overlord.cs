using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace SOSG.Stage
{
    public class Overlord : MonoBehaviour
    {
        [Header("Event SO - Listener")]
        [SerializeField] private IntEventSO gashaponMergedEventSO;
        [SerializeField] private VoidEventSO monsterSummonEventSO;

        [Header("Variable SO - Setter")]
        [SerializeField] private Vector3VarSO moveSpeedVarSO;

        [Header("Component")]
        [SerializeField] private Animator ani;

        public static float XPos;

        private void Awake()
        {
            XPos = transform.position.x;
        }

        private void OnEnable()
        {
            monsterSummonEventSO.OnEventRaised += OnMonsterSummoned;
        }
        
        private void OnDisable()
        {
            monsterSummonEventSO.OnEventRaised -= OnMonsterSummoned;
        }

        private void OnMonsterSummoned()
        {
            ani.SetTrigger(AnimatorCache.SummonTrigger);
        }

        private async UniTaskVoid PlaySummonAnimation()
        {
            ani.SetTrigger(AnimatorCache.SummonTrigger);
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
        }
    }
}