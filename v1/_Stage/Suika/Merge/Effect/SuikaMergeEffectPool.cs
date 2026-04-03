using System;
using System.Collections.Generic;
using SOSG.Monster;
using UnityEngine;

namespace SOSG.Stage.Suika.Merge
{
    public class SuikaMergeEffectPool : MonoBehaviour
    {
        [SerializeField] private SuikaMergeEffectPoolSO poolSO;

        [SerializeField] private SuikaMergeEffectBase commonEffectPrefab;
        [SerializeField] private SuikaMergeEffectBase uncommonEffectPrefab;
        [SerializeField] private SuikaMergeEffectBase rareEffectPrefab;
        [SerializeField] private SuikaMergeEffectBase epicEffectPrefab;

        private Stack<SuikaMergeEffectBase> _commonPool;
        private Stack<SuikaMergeEffectBase> _uncommonPool;
        private Stack<SuikaMergeEffectBase> _rarePool;
        private Stack<SuikaMergeEffectBase> _epicPool;

        private void Awake()
        {
            _commonPool = new Stack<SuikaMergeEffectBase>();
            _uncommonPool = new Stack<SuikaMergeEffectBase>();
            _rarePool = new Stack<SuikaMergeEffectBase>();
            _epicPool = new Stack<SuikaMergeEffectBase>();
        }

        private void OnEnable()
        {
            poolSO.GetRequsted += OnGetRequested;
            poolSO.ReturnRequseted += OnReturnRequested;
        }

        private void OnDisable()
        {
            poolSO.GetRequsted -= OnGetRequested;
            poolSO.ReturnRequseted -= OnReturnRequested;
        }

        private SuikaMergeEffectBase OnGetRequested(MonsterGrade grade, Vector3 pos)
        {
            var pool = grade switch
            {
                MonsterGrade.Common => _commonPool,
                MonsterGrade.Uncommon => _uncommonPool,
                MonsterGrade.Rare => _rarePool,
                MonsterGrade.Epic => _epicPool,
                _ => throw new ArgumentOutOfRangeException()
            };

            if (pool.TryPop(out var effect) is false)
            {
                effect = grade switch
                {
                    MonsterGrade.Common => Instantiate(commonEffectPrefab, pos, Quaternion.identity, transform),
                    MonsterGrade.Uncommon => Instantiate(uncommonEffectPrefab, pos, Quaternion.identity, transform),
                    MonsterGrade.Rare => Instantiate(rareEffectPrefab, pos, Quaternion.identity, transform),
                    MonsterGrade.Epic => Instantiate(epicEffectPrefab, pos, Quaternion.identity, transform),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
            else
            {
                effect.transform.position = pos;
                effect.gameObject.SetActive(true);
            }

            return effect;
        }

        private void OnReturnRequested(SuikaMergeEffectBase effect)
        {
            switch (effect.Grade)
            {
                case MonsterGrade.Common:
                    _commonPool.Push(effect);
                    break;
                case MonsterGrade.Uncommon:
                    _uncommonPool.Push(effect);
                    break;
                case MonsterGrade.Rare:
                    _rarePool.Push(effect);
                    break;
                case MonsterGrade.Epic:
                    _epicPool.Push(effect);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}