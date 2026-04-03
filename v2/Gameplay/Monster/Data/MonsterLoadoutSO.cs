using System;
using TBM.MaouSuika.Core;
using TBM.MaouSuika.Gameplay.Monster;
using UnityEngine;

namespace TBM.MaouSuika.Data
{
    [CreateAssetMenu(fileName = "MonsterLoadout", menuName = "TBM/Monster/Loadout")]
    public class MonsterLoadoutSO : ScriptableObject
    {
        [field: SerializeField] public MonsterDataSO[] monsterData { get; private set; }

        public int Count => monsterData.Length;


#if UNITY_EDITOR
        [TextArea] public string description;

        private void OnValidate()
        {
            if (monsterData == null)
            {
                Debug.LogError("Monster Data is null.");
                return;
            }

            if (monsterData.Length != GameRule.Puzzle.Suika.TIER_COUNT)
            {
                Debug.LogError("Monster Data size is not matched.");
                return;
            }

            for (var i = 0; i < monsterData.Length; i++)
            {
                if (monsterData[i] == null)
                {
                    Debug.LogError($"Monster Data[{i}] is null.");
                }
            }
        }
#endif
    }
}