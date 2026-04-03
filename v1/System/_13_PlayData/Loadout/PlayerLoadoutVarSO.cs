using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using SOSG.Monster;
using UnityEngine;
using UnityEngine.Serialization;

namespace SOSG.System.PlayData
{
    [CreateAssetMenu(fileName = "PlayerLoadoutVarSO", menuName = "TaeBoMi/Play Data/Loadout")]
    public class PlayerLoadoutVarSO : ScriptableObject
    {
        public int playerNum;
        public PlayerLoadout data;
        public MonsterLoadout monsterLoadout;
        // todo skill

        public async UniTask SetUp(PlayerLoadoutString playerLoadoutString)
        {
            monsterLoadout = await MonsterLoadout.ConvertFrom(playerLoadoutString.monsterLoadoutString);
        }

        public void SetLoadout(PlayerLoadout loadout)
        {
            data = loadout;
        }

        public void ChangeMonsterLoadout(int tier, MonsterDataSO newMonsterData) => monsterLoadout.Change(tier, newMonsterData);

    }
}