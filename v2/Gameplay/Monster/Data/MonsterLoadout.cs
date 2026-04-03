using System;
using System.Collections.Generic;
using System.Linq;
using TBM.MaouSuika.Core;
using TBM.MaouSuika.Gameplay.Monster;

namespace TBM.MaouSuika.Data
{
    public class MonsterLoadout
    {
        public int Count => _monsters.Length;

        private readonly MonsterDataSO[] _monsters;

        public MonsterDataSO this[int tier]
        {
            get
            {
                if (tier < 0 || tier >= _monsters.Length) throw new ArgumentOutOfRangeException(nameof(tier));
                return _monsters[tier];
            }
        }

        public MonsterLoadout(IEnumerable<MonsterDataSO> monsters)
        {
            if (monsters == null) throw new ArgumentNullException(nameof(monsters));

            _monsters = monsters.ToArray();
            if (_monsters.Length != GameRule.Puzzle.Suika.TIER_COUNT)
                throw new ArgumentException($"Invalid Count[{_monsters.Length}]");

            for (var i = 0; i < _monsters.Length; i++)
            {
                if (_monsters[i] == null) throw new ArgumentNullException($"Null at tier[{i}]");
            }
        }
    }
}