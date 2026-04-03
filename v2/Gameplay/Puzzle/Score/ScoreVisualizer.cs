using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    public class ScoreVisualizer : MonoBehaviour
    {
        [SerializeField] private MainScoreCounter mainScoreCounter;

        public void Setup(int initialScore = 0)
        {
            mainScoreCounter.SetScore(initialScore);
        }

        public void UpdateMainScore(int score)
        {
            mainScoreCounter.SetScore(score);
        }
    }
}