using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    public class SuikaObjectFactory : MonoBehaviour
    {
        [SerializeField] private MonsterVisualFactory monsterFactory;

        [SerializeField] private SuikaObject suikaObjectPrefab;

        private IObjectPool<SuikaObject> _suikaObjectPool;
        private SuikaTierDataTable _tierDataTable;

        private MergeSystem _mergeSystem;

        public void Initialize(MergeSystem mergeSystem, SuikaTierDataTable tierDataTable)
        {
            _mergeSystem = mergeSystem;
            _tierDataTable = tierDataTable;
            _suikaObjectPool = new ObjectPool<SuikaObject>(
                CreateSuika, OnGetSuika, OnReleaseSuika, OnDestroySuika,
                defaultCapacity: 40, maxSize: 300);
            monsterFactory.Initialize(tierDataTable);
        }

        public SuikaObject Get(int tier, int creationOrder)
        {
            if (tier < 0 || tier >= _tierDataTable.Count) return null;

            var suikaObject = _suikaObjectPool.Get();
            var monsterVisual = monsterFactory.Get(tier);

            var setupData = new SuikaObjectSetupData(_tierDataTable[tier], creationOrder);
            suikaObject.Setup(setupData, monsterVisual);

            return suikaObject;
        }

        public void Release(SuikaObject suikaObject)
        {
            var monsterVisual = suikaObject.MonsterVisual;
            if (monsterVisual != null)
            {
                if (monsterFactory != null)
                {
                    monsterFactory.Release(suikaObject.Tier, monsterVisual);
                }
                else
                {
                    Destroy(monsterVisual.gameObject);
                }
            }

            if (_suikaObjectPool == null)
            {
                Destroy(suikaObject.gameObject);
                return;
            }

            _suikaObjectPool.Release(suikaObject);
        }


        private SuikaObject CreateSuika()
        {
            var suika = Instantiate(suikaObjectPrefab, transform);
            suika.Initialize(_mergeSystem);
            return suika;
        }

        private void OnGetSuika(SuikaObject suika)
        {
            suika.gameObject.SetActive(true);
        }

        private void OnReleaseSuika(SuikaObject suika)
        {
            suika.gameObject.SetActive(false);
            suika.transform.SetParent(transform);
        }

        private void OnDestroySuika(SuikaObject item)
        {
            if (!item) return;

            Destroy(item.gameObject);
        }
    }
}