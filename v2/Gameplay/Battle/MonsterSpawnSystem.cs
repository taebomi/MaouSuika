using TBM.MaouSuika.Data;
using TBM.MaouSuika.Gameplay.Monster;
using Unity.Mathematics;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Battle
{
    public class MonsterSystem : MonoBehaviour
    {
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private Transform monsterContainer;

        private MonsterLoadout _loadout;

        public void Initialize(MonsterLoadout loadout)
        {
            _loadout = loadout;
        }

        public void Spawn(int tier, BattleSide side)
        {
            var prefab = _loadout[tier].battlePrefab;
            var monster = Instantiate(prefab, spawnPoint.position, quaternion.identity, monsterContainer);
            monster.Setup(tier, side);
            monster.Move();
        }

        public void PlayVictoryAll(MonsterVictoryType type)
        {
            foreach (var visual in monsterContainer.GetComponentsInChildren<MonsterVisualController>())
                visual.PlayVictory(type);
        }

        public void StopVictoryAll()
        {
            foreach (var visual in monsterContainer.GetComponentsInChildren<MonsterVisualController>())
                visual.StopVictory();
        }
    }
}
