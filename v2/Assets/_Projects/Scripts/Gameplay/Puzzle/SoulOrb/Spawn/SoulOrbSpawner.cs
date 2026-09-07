using System;
using System.Collections.Generic;
using TBM.Pool;
using UnityEngine;

namespace MaouSuika.Gameplay
{
    public partial class SoulOrbSpawner : MonoBehaviour, ISoulOrbSpawner
    {
        private const int PREWARM_COUNT = 75;

        [SerializeField] private SoulOrbTierDefinitionSO definition;
        [SerializeField] private SoulOrb orbPrefab;

        private MonsterLoadout _loadout;
        private MonoPool<SoulOrb> _orbPool;
        private readonly List<SoulOrb> _activeOrbs = new();

        private int _creationOrder;

        public IReadOnlyList<SoulOrb> ActiveOrbs => _activeOrbs;

        public void Initialize(MonsterLoadout loadout, Func<SoulOrb, SoulOrb, bool> tryMerge)
        {
            _loadout = loadout;
            _orbPool = new MonoPool<SoulOrb>(orbPrefab, transform, PREWARM_COUNT,
                orb => orb.Initialize(tryMerge), orb => orb.OnDespawn());
        }

        public void Setup()
        {
            _creationOrder = 0;
        }

        public SoulOrb Spawn(int tier, Vector3 position, int creationOrder, bool hasLanded, SoulOrbSpawnMode spawnMode)
        {
            var soulOrb = _orbPool.Get();

            var monsterData = _loadout[tier];

            var setupData = new SoulOrbSetupData(tier: tier,
                scale: definition.GetScale(tier),
                creationOrder: creationOrder,
                hasLanded: hasLanded,
                spawnMode: spawnMode, monsterData: monsterData);

            soulOrb.Setup(setupData);
            soulOrb.transform.position = position;
            soulOrb.gameObject.SetActive(true);

            _activeOrbs.Add(soulOrb);
            return soulOrb;
        }

        public SoulOrb Spawn(int tier, Vector3 position, bool hasLanded, SoulOrbSpawnMode spawnMode)
        {
            return Spawn(tier, position, ++_creationOrder, hasLanded, spawnMode);
        }

        public void Despawn(SoulOrb orb)
        {
            _activeOrbs.Remove(orb);
            _orbPool.Release(orb);
        }
    }
}