using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    [CreateAssetMenu(fileName = "PuzzleGameOverSystemConfig SO", menuName = "Maou Suika/Puzzle/GameOver/Config")]
    public class PuzzleGameOverSystemConfigSO : ScriptableObject
    {
        [field: Header("GameOver Timer")]
        [field: SerializeField] public float GameOverTimerDuration { get; private set; } = 5f;

        [field: Header("Warning Sensor")]
        [field: SerializeField] public float warningSetThreshold { get; private set; } = 1.5f;
        [field: SerializeField] public float warningUnsetThreshold { get; private set; } = 0.75f;

        [field: Header("Critical Sensor")]
        [field: SerializeField] public float CriticalSetThreshold { get; private set; } = 1.0f;
    }
}