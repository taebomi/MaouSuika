using System;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    public class ScoreSystem : MonoBehaviour
    {
        [SerializeField] private ScoreVisualizer visualizer;

        private ScoreModel _scoreModel;

        public event Action<int> ScoreChanged;
        
        // todo Main Score Counter 연출
        public void Initialize(SuikaTierDataTable tierDataTable)
        {
            _scoreModel = new ScoreModel(tierDataTable);
        }

        public void Setup(int initialScore = 0)
        {
            _scoreModel.Setup(initialScore);
            visualizer.Setup(initialScore);
        }

        public void HandleSuikaMerged(MergeEvent mergeEvent)
        {
            visualizer.UpdateMainScore(_scoreModel.CurrentScore);
            ScoreChanged?.Invoke(_scoreModel.CurrentScore);
        }
    }
}