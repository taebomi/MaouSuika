using Cysharp.Threading.Tasks;
using SOSG.System.PlayData;
using UnityEngine;

namespace SOSG.System.Loadout
{
    [CreateAssetMenu(menuName = "SOSG/Loadout/Player Loadout")]
    public class PlayerLoadoutSO : ScriptableObject
    {
        public int playerNum;
        public MonsterLoadoutSO monsterLoadout;
    }
}