using System;
using System.Linq;

namespace MaouSuika.Gameplay
{
    public class MonsterLoadout
    {
        private readonly MonsterDataSO[] _monstersByTier;

        public int Length => _monstersByTier.Length;

        public MonsterDataSO this[int tier] => _monstersByTier[tier];

        public MonsterLoadout(MonsterDataSO[] monstersByTier)
        {
            if (monstersByTier == null) throw new ArgumentNullException();
            if (monstersByTier.Length != SoulOrbTiers.COUNT) throw new Exception();
            if (monstersByTier.Any(monsterData => !monsterData)) throw new ArgumentNullException();

            _monstersByTier = monstersByTier;
        }
    }
}