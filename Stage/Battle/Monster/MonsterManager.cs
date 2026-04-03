using Cysharp.Threading.Tasks;
using SOSG.Monster;
using SOSG.Stage.Suika.Combo;
using SOSG.System;
using SOSG.System.PlayData;
using SOSG.System.Scene;
using TaeBoMi.Pool;
using UnityEngine;

namespace SOSG.Stage
{
    public class MonsterManager : MonoBehaviour
    {
        [SerializeField] private PlayerLoadoutVarSO loadoutVarSO;
        [SerializeField] private GashaponComboSO curComboVarSO;
        [SerializeField] private IntEventSO gashaponMergeEventSO;

        [SerializeField] private VoidEventSO monsterSummonEventSO;

        [SerializeField] private MonsterController[] monsterArr;

        [SerializeField] private SummonEffect summonEffectPrefab;

        private bool[] _isMonsterActivated;

        private IObjectPool<SummonEffect> _summonEffectPool;

        private void Awake()
        {
            SceneSetUpHelper.AddTask(SetUp);
        }

        private void OnEnable()
        {
            gashaponMergeEventSO.OnEventRaised += OnGashaponMerged;
        }

        private void OnDisable()
        {
            gashaponMergeEventSO.OnEventRaised -= OnGashaponMerged;
        }

        private void SetUp()
        {
            _isMonsterActivated = new bool[monsterArr.Length];
            for (var idx = 0; idx < monsterArr.Length; idx++)
            {
                monsterArr[idx].Initialize(loadoutVarSO.data.MonsterLoadout[idx]);
                _isMonsterActivated[idx] = false;
            }

            _summonEffectPool = new ObjectPool<SummonEffect>(CreateSummonEffect, 5);

            return;

            SummonEffect CreateSummonEffect()
            {
                var effect = Instantiate(summonEffectPrefab, transform);
                effect.Initialize(_summonEffectPool);
                return effect;
            }
        }

        private void OnGashaponMerged(int idx)
        {
            if (_isMonsterActivated[idx] is false)
            {
                _isMonsterActivated[idx] = true;
                monsterArr[idx].Summon();
                var summonEffect = _summonEffectPool.Get();
                summonEffect.Set(monsterArr[idx].GetFootPosition());
                monsterSummonEventSO.RaiseEvent();
                return;
            }

            if (curComboVarSO.Grade >= ComboGrade.High)
            {
                monsterArr[idx].RotateJump();
            }
            else
            {
                monsterArr[idx].Jump();
            }
        }
    }
}