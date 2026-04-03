using TaeBoMi.Pool;
using UnityEngine;

namespace SOSG.Stage
{
    public class ComboSystem : MonoBehaviour
    {
        [Header("Event SO - broadcaster")]
        [SerializeField] private VoidEventSO comboFailedEventSO;
        
        [Header("Event SO - Listener")]
        [SerializeField] private VoidEventSO shootEventSO;
        [SerializeField] private GashaponMergeEventSO mergeEventSO;


        [Header("Variable SO - Setter")]
        [SerializeField] private GashaponComboSO curCombo; // 현재 콤보
        [SerializeField] private GashaponComboSO lastCombo; // 마지막 콤보
        [SerializeField] private GashaponComboSO highestCombo; // 최고 콤보

        [Header("Component")]
        [SerializeField] private ComboEffect comboEffectPrefab;

        
        private IObjectPool<ComboEffect> comboEffectPool;

        private bool _hasMergedThisTurn;
        
        private void Awake()
        {
            Initialize();

            mergeEventSO.OnEventRaised += OnMerged;
            shootEventSO.OnEventRaised += OnShot;
        }

        private void OnDestroy()
        {
            mergeEventSO.OnEventRaised -= OnMerged;
            shootEventSO.OnEventRaised -= OnShot;
        }

        private void Initialize()
        {
            lastCombo.Initialize(0);
            curCombo.Initialize(0);
            highestCombo.Initialize(0);

            comboEffectPool = new ObjectPool<ComboEffect>(() =>
            {
                var comboEffect = Instantiate(comboEffectPrefab, transform);
                comboEffect.Initialize(comboEffectPool);
                return comboEffect;
            });
        }


        private void OnMerged(Gashapon capsule1, Gashapon capsule2)
        {
            _hasMergedThisTurn = true;
            curCombo.Increase();
            if (curCombo.IsCombo)
            {
                var effectPos = (capsule1.transform.position + capsule2.transform.position) * 0.5f;
                var comboEffect = comboEffectPool.Get();
                comboEffect.Set(effectPos, curCombo.Value);
            }
        }

        private void OnShot()
        {
            if (!_hasMergedThisTurn)
            {
                comboFailedEventSO.RaiseEvent();
                lastCombo.Set(curCombo.Value);
                if (highestCombo.Value < curCombo.Value)
                {
                    highestCombo.Set(curCombo.Value);
                }
                curCombo.Refresh();
            }
            
            _hasMergedThisTurn = false;
        }
    }
}