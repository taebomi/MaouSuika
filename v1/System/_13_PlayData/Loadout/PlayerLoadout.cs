using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace SOSG.System.PlayData
{
    [Serializable]
    public class PlayerLoadout
    {
        [field: SerializeField] public MonsterLoadout MonsterLoadout { get; private set; } = new();

        public static async UniTask<PlayerLoadout> CreateDefaultInstance()
        {
            var instance = new PlayerLoadout
            {
                MonsterLoadout = await MonsterLoadout.CreateDefaultInstance()
            };

            return instance;
        }

        public static async UniTask<PlayerLoadout> ConvertFrom(PlayerLoadoutString loadoutString)
        {
            var loadout = new PlayerLoadout();
            loadout.MonsterLoadout = await MonsterLoadout.ConvertFrom(loadoutString.monsterLoadoutString);
            return loadout;
        }
    }
}