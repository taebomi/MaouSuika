using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using SOSG.Monster;
using TaeBoMi;
using UnityEngine;

namespace SOSG.System.PlayData
{
    [Serializable]
    [ES3Serializable]
    public class MonsterLoadout
    {
        [field: SerializeField] public MonsterDataSO[] Data { get; private set; } = new MonsterDataSO[Size];

        public const int Size = 11;

        public MonsterDataSO this[int tier]
        {
            get => Data[tier];
            set => Data[tier] = value;
        }

        public static async UniTask<MonsterLoadout> CreateDefaultInstance()
        {
            var instance = new MonsterLoadout();
            var defaultLoadoutString = MonsterLoadoutString.CreateDefaultInstance();

            for (var tier = 0; tier < Size; tier++)
            {
                var dataSO = await MonsterDataSOLoader.LoadMonsterDataSOAsync(defaultLoadoutString[tier]);
                if (dataSO is null)
                {
                    TBMUtility.LogError($"# Monster Loadout - Default Initialize Failed");
                    TBMUtility.LogError($"## Cannot Find Monster Data : {defaultLoadoutString[tier]}");
                    return null;
                }

                instance.Data[tier] = dataSO;
            }

            return instance;
        }

        public static async UniTask<MonsterLoadout> ConvertFrom(MonsterLoadoutString loadoutString)
        {
            var instance = new MonsterLoadout();
            for (var tier = 0; tier < Size; tier++)
            {
                var dataSO = await MonsterDataSOLoader.LoadMonsterDataSOAsync(loadoutString[tier]);
                if (dataSO is null)
                {
                    TBMUtility.LogError($"# Monster Loadout - Convert From Failed");
                    TBMUtility.LogError($"## Cannot Find Monster Data : {loadoutString[tier]}");
                    return null;
                }

                instance.Data[tier] = dataSO;
            }

            return instance;
        }

        public void Change(List<MonsterDataSO> monsterDataList)
        {
            if (monsterDataList.Count != Size)
            {
                Debug.LogError($"# Monster Loadout - Change Failed");
                Debug.LogError($"## Monster Data List Count Mismatch : {monsterDataList.Count}");
                return;
            }

            for (var tier = 0; tier < Size; tier++)
            {
                Data[tier] = monsterDataList[tier];
            }
        }

        public void Change(int tier, MonsterDataSO newMonsterData) => Data[tier] = newMonsterData;
    }
}