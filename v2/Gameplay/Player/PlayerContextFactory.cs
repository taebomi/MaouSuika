using TBM.MaouSuika.Data;
using TBM.MaouSuika.Gameplay.Monster;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Player
{
    [CreateAssetMenu(fileName = "PlayerContextFactory", menuName = "TBM/Player/Context Factory")]
    public class PlayerContextFactorySO : ScriptableObject
    {
        [SerializeField] private MonsterDatabaseSO monsterDB;

        public PlayerContext Create(int playerIndex, MonsterLoadoutData loadoutData)
        {
            var monsterLoadout = loadoutData.ToRuntimeLoadout(monsterDB);
            return new PlayerContext(playerIndex, monsterLoadout);
        }
    }
}