using System;
using System.Collections.Generic;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    /// <summary>
    /// 플레이어의 다음 Suika Tier 목록
    /// </summary>
    public class SuikaQueue
    {
        public int Count => _tiers.Count;
        public IReadOnlyList<int> Items => _tiers;

        public event Action<IReadOnlyList<int>> QueueChanged;

        private readonly SuikaDeck _deck;
        private readonly List<int> _tiers;
        private readonly int _queueSize;

        public SuikaQueue(SuikaDeck deck, int queueSize)
        {
            _deck = deck;
            _tiers = new List<int>(queueSize + 2);
            _queueSize = queueSize;
        }

        public void Setup()
        {
            _tiers.Clear();
            for (var i = 0; i < _queueSize; i++)
            {
                _tiers.Add(_deck.GetTier());
            }
        }

        public int Dequeue()
        {
            if (_tiers.Count == 0) throw new InvalidOperationException($"{nameof(SuikaQueue)} is empty.)");

            var tier = _tiers[0];
            _tiers.RemoveAt(0);
            _tiers.Add(_deck.GetTier());
            QueueChanged?.Invoke(_tiers);
            return tier;
        }
    }
}