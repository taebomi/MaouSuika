using System;
using TBM.MaouSuika.Gameplay.Battle;
using TBM.MaouSuika.Gameplay.Puzzle;

namespace TBM.MaouSuika.Gameplay
{
    [Serializable]
    public struct RegionProgressionEntry
    {
        public int scoreThreshold;
        public BattleRegion battleRegion;
        public PuzzleRegion puzzleRegion;
    }
}
