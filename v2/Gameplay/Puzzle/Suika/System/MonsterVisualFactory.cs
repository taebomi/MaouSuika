using System.Collections.Generic;
using TBM.MaouSuika.Gameplay.Monster;
using UnityEngine;
using UnityEngine.Pool;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    /// <summary>
    /// Suika Object Pooling 용도로만 사용.
    /// 그림자 끄도록 되어있으니 범용적으로 사용 시 수정 필요함.
    /// </summary>
    public class MonsterVisualFactory : MonoBehaviour
    {
        private IObjectPool<MonsterVisualController>[] _pools;

        public void Initialize(SuikaTierDataTable tierDataTable)
        {
            _pools = new IObjectPool<MonsterVisualController>[tierDataTable.Count];
            for (var tier = 0; tier < tierDataTable.Count; tier++)
            {
                _pools[tier] = CreatePool(tierDataTable[tier].MonsterData.visualPrefab);
            }
        }

        public MonsterVisualController Get(int tier)
        {
            if (_pools == null || !IsValidTier(tier))
            {
                return null;
            }

            return _pools[tier].Get();
        }

        public void Release(int tier, MonsterVisualController visualController)
        {
            if (_pools == null || !IsValidTier(tier))
            {
                Destroy(visualController.gameObject);
                return;
            }

            _pools[tier].Release(visualController);
        }

        private bool IsValidTier(int tier)
        {
            return tier >= 0 && tier < _pools.Length;
        }

        private IObjectPool<MonsterVisualController> CreatePool(MonsterVisualController item)
        {
            return new ObjectPool<MonsterVisualController>(
                () => CreateItem(item),
                OnGetItem,
                OnReleaseItem,
                OnDestroyItem,
                maxSize: 100);
        }

        private MonsterVisualController CreateItem(MonsterVisualController item)
        {
            var monster = Instantiate(item, transform);
            monster.SetShadowActive(false);
            return monster;
        }

        private void OnGetItem(MonsterVisualController item)
        {
            item.gameObject.SetActive(true);
        }

        private void OnReleaseItem(MonsterVisualController item)
        {
            item.gameObject.SetActive(false);
            item.transform.SetParent(transform);
        }

        private void OnDestroyItem(MonsterVisualController item)
        {
            if (!item) return;

            Destroy(item.gameObject);
        }
    }
}