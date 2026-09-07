using System;

namespace MaouSuika.Gameplay
{
    public class SoulOrbDeck
    {
        private readonly Random _random;
        private readonly int _drawableTierCount;

        public SoulOrbDeck(int seed, int drawableTierCount)
        {
            _random = new Random(seed);
            _drawableTierCount = drawableTierCount;
        }

        public int Draw()
        {
            return _random.Next(_drawableTierCount);
        }
    }
}