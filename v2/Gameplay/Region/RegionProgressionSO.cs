using TBM.MaouSuika.Gameplay.Battle;
using TBM.MaouSuika.Gameplay.Puzzle;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay
{
    [CreateAssetMenu(fileName = "RegionProgressionSO", menuName = "Maou Suika/Region/Progression")]
    public partial class RegionProgressionSO : ScriptableObject
    {
        [SerializeField] private BattleRegion initialBattle;
        [SerializeField] private PuzzleRegion initialPuzzle;
        [SerializeField] private RegionProgressionEntry[] entries;

        public BattleRegion InitialBattle => initialBattle;
        public PuzzleRegion InitialPuzzle => initialPuzzle;

        public BattleRegion GetBattleRegion(int score)
        {
            var result = initialBattle;
            foreach (var entry in entries)
            {
                if (score < entry.scoreThreshold) break;
                result = entry.battleRegion;
            }
            return result;
        }

        public PuzzleRegion GetPuzzleRegion(int score)
        {
            var result = initialPuzzle;
            foreach (var entry in entries)
            {
                if (score < entry.scoreThreshold) break;
                result = entry.puzzleRegion;
            }
            return result;
        }
    }
}
