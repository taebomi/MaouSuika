using System;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    /// <summary>
    /// 플레이어의 Suika Queue를 채우기 위한 Deck.
    /// </summary>
    public class SuikaDeck
    {
        private readonly Random _random;

        private readonly int _minTier;
        private readonly int _maxTier;

        public SuikaDeck(int minTier, int maxTier)
        {
            _minTier = minTier;
            _maxTier = maxTier;

            _random = new Random();
        }

        public SuikaDeck(int minTier, int maxTier, int seed)
        {
            _minTier = minTier;
            _maxTier = maxTier;

            _random = new Random(seed);
        }

        public int GetTier()
        {
            return _random.Next(_minTier, _maxTier + 1);
        }
    }
}