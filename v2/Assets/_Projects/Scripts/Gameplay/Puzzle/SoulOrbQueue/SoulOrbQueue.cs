using System;
using System.Collections.Generic;
using R3;

namespace MaouSuika.Gameplay
{
    public class SoulOrbQueue : IDisposable
    {
        public const int LOOKAHEAD = 5;

        private readonly List<int> _tiers = new(LOOKAHEAD);
        private readonly Func<int> _nextTierSelector;

        public IReadOnlyList<int> Tiers => _tiers;

        private readonly Subject<Unit> _changed = new();
        public Observable<Unit> Changed => _changed;

        public SoulOrbQueue(Func<int> nextTierSelector)
        {
            _nextTierSelector = nextTierSelector ?? throw new ArgumentNullException(nameof(nextTierSelector));
            for (var i = 0; i < LOOKAHEAD; i++) _tiers.Add(_nextTierSelector());
        }

        public int Dequeue()
        {
            var tier = _tiers[0];
            _tiers.RemoveAt(0);
            _tiers.Add(_nextTierSelector());
            _changed.OnNext(Unit.Default);
            return tier;
        }

        public void Dispose()
        {
            _changed?.Dispose();
        }
    }
}