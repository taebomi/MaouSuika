using System;
using System.Collections.Generic;
using TBM.MaouSuika.Gameplay.Unit;
using UnityEngine;
using UnityEngine.Pool;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    public class SuikaMergeEffectPool : MonoBehaviour
    {
        [SerializeField] private MergeEffectBase commonEffectPrefab;
        [SerializeField] private MergeEffectBase uncommonEffectPrefab;
        [SerializeField] private MergeEffectBase rareEffectPrefab;
        [SerializeField] private MergeEffectBase epicEffectPrefab;

        private IObjectPool<MergeEffectBase> _commonPool;
        private IObjectPool<MergeEffectBase> _uncommonPool;
        private IObjectPool<MergeEffectBase> _rarePool;
        private IObjectPool<MergeEffectBase> _epicPool;

        // TODO 딕셔너리로 전환
        private readonly Dictionary<MergeEffectGrade, IObjectPool<MergeEffectBase>> _pools = new();

        private void Awake()
        {
            _commonPool = new ObjectPool<MergeEffectBase>(CreateCommonEffect, OnGetMergeEffect, OnReleaseMergeEffect,
                defaultCapacity: 3);
            _uncommonPool = new ObjectPool<MergeEffectBase>(CreateUncommonEffect, OnGetMergeEffect,
                OnReleaseMergeEffect, defaultCapacity: 3);
            _rarePool = new ObjectPool<MergeEffectBase>(CreateRareEffect, OnGetMergeEffect, OnReleaseMergeEffect,
                defaultCapacity: 3);
            _epicPool = new ObjectPool<MergeEffectBase>(CreateEpicEffect, OnGetMergeEffect, OnReleaseMergeEffect,
                defaultCapacity: 3);
        }

        private MergeEffectBase CreateCommonEffect()
        {
            var effect = Instantiate(commonEffectPrefab, transform);
            effect.Initialize(_commonPool);
            return effect;
        }

        private MergeEffectBase CreateUncommonEffect()
        {
            var effect = Instantiate(uncommonEffectPrefab, transform);
            effect.Initialize(_uncommonPool);
            return effect;
        }

        private MergeEffectBase CreateRareEffect()
        {
            var effect = Instantiate(rareEffectPrefab, transform);
            effect.Initialize(_rarePool);
            return effect;
        }

        private MergeEffectBase CreateEpicEffect()
        {
            var effect = Instantiate(epicEffectPrefab, transform);
            effect.Initialize(_epicPool);
            return effect;
        }

        public MergeEffectBase Get(MergeEffectGrade grade)
        {
            var pool = grade switch
            {
                MergeEffectGrade.Common => _commonPool,
                MergeEffectGrade.Uncommon => _uncommonPool,
                MergeEffectGrade.Rare => _rarePool,
                MergeEffectGrade.Epic => _epicPool,
                _ => throw new ArgumentOutOfRangeException(nameof(grade), grade, null)
            };
            var effect = pool.Get();
            return effect;
        }

        private void OnGetMergeEffect(MergeEffectBase mergeEffect)
        {
            mergeEffect.gameObject.SetActive(true);
        }

        private void OnReleaseMergeEffect(MergeEffectBase mergeEffect)
        {
            mergeEffect.gameObject.SetActive(false);
        }
    }
}