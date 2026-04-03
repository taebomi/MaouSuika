using System;
using System.Collections.Generic;
using TBM.MaouSuika.Data;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Monster
{
    [CreateAssetMenu(fileName = "MonsterDatabaseSO", menuName = "TBM/Monster/Database")]
    public class MonsterDatabaseSO : ScriptableObject
    {
        [SerializeField] private List<MonsterDataSO> monsters;
        [SerializeField] private MonsterLoadoutSO defaultLoadout;

        [NonSerialized] private Dictionary<string, MonsterDataSO> _monsterDict;

        [NonSerialized] private bool _isInitialized;

        public MonsterDataSO this[string id] => GetMonsterData(id);


        public void Initialize()
        {
            if (_isInitialized) return;

            monsters ??= new List<MonsterDataSO>();
            _monsterDict = new Dictionary<string, MonsterDataSO>(monsters.Count);

            foreach (var monsterData in monsters)
            {
                if (monsterData == null) continue;

                if (string.IsNullOrEmpty(monsterData.id))
                {
                    Logger.Error($"id is empty for monster {monsterData.name}");
                    continue;
                }

                if (!_monsterDict.TryAdd(monsterData.id, monsterData))
                {
                    Logger.Error($"Duplicate ID detected : {monsterData.id}");
                    continue;
                }
            }

            _isInitialized = true;
        }

        public MonsterDataSO GetMonsterData(string id)
        {
            if (!_isInitialized) Initialize();
            if (_monsterDict.TryGetValue(id, out var monsterData))
            {
                return monsterData;
            }

            Logger.Error($"Monster with id {id} is not found");
            return null;
        }

        public MonsterLoadout CreateDefaultLoadout()
        {
            if (defaultLoadout == null) throw new InvalidOperationException("defaultLoadout is null");

            return new MonsterLoadout(defaultLoadout.monsterData);
        }

#if UNITY_EDITOR

        private void OnValidate()
        {
        }

#endif
    }
}