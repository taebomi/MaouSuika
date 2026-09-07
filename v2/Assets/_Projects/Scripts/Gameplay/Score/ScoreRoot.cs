using System;
using R3;
using UnityEngine;

namespace MaouSuika.Gameplay
{
    public class ScoreRoot : MonoBehaviour
    {
        [SerializeField] private ScoreView scoreView;

        private Score _score;

        public int CurrentScore => _score.Current;
        public Observable<int> TotalScoreChanged => _score.TotalAfterAdded;

        public void Initialize()
        {
            scoreView.Initialize();
        }

        public void Setup()
        {
            _score?.Dispose();
            _score = new Score();
            scoreView.Setup(_score.TotalAfterAdded, _score.Current);
        }

        private void OnDestroy()
        {
            _score?.Dispose();
        }

        public void Add(int amount)
        {
            _score.Add(amount);
        }
    }
}