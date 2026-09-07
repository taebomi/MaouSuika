using System;
using System.Collections.Generic;
using R3;

namespace MaouSuika.Gameplay
{
    public class SoulOrbQueueViewModel : IDisposable
    {
        // Preview Count 범위 및 기본값 지정, 이후 시스템 확장 시 사용
        private const int MIN_PREVIEW_COUNT = 0;
        private const int MAX_PREVIEW_COUNT = 5;
        private const int DEFAULT_PREVIEW_COUNT = 2;

        private readonly SoulOrbQueue _queue;
        private readonly MonsterLoadout _monsterLoadout;

        private readonly ReactiveProperty<int> _previewCount;

        public IReadOnlyList<int> Tiers => _queue.Tiers;
        public int PreviewCount => _previewCount.Value;

        public Observable<Unit> RedrawRequested { get; }

        public SoulOrbQueueViewModel(SoulOrbQueue queue, MonsterLoadout monsterLoadout)
        {
            _queue = queue;
            _monsterLoadout = monsterLoadout;

            _previewCount = new ReactiveProperty<int>(DEFAULT_PREVIEW_COUNT);

            RedrawRequested = Observable.Merge(_queue.Changed, _previewCount.AsUnitObservable());
        }

        public SoulOrbQueueSlotData GetSlotData(int index)
        {
            var tier = _queue.Tiers[index];
            var monster = _monsterLoadout[tier];

            return new SoulOrbQueueSlotData(tier, monster.icon, monster.soulOrbColor);
        }

        public void Dispose()
        {
            _previewCount.Dispose();
        }
    }
}