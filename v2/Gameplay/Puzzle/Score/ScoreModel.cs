using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    public class ScoreModel
    {
        private ObscuredInt _currentScore;
        public int CurrentScore => _currentScore;

        private readonly SuikaTierDataTable _suikaTierDataTable;

        public ScoreModel(SuikaTierDataTable tierDataTable)
        {
            _currentScore = 0;
            _suikaTierDataTable = tierDataTable;
        }

        public void Setup(int score)
        {
            _currentScore = score;
        }

        /// <summary>
        /// tier에 해당하는 score 추가
        /// </summary>
        /// <returns>획득한 점수</returns>
        public int AddScore(int tier)
        {
            if (!_suikaTierDataTable.TryGetTierData(tier, out var tierData))
            {
                Logger.Warning($"Tier[{tier}] is out of range.");
                return 0;
            }

            var score = tierData.Score;

            _currentScore += score;
            return score;
        }
    }
}