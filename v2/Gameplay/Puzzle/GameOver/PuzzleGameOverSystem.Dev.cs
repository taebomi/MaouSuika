#if UNITY_EDITOR

using Sirenix.OdinInspector;


namespace TBM.MaouSuika.Gameplay.Puzzle
{
    public partial class PuzzleGameOverSystem
    {
        [BoxGroup("Dev")] [ShowInInspector] private DangerLevel Dev_CurrentDangerLevel => _currentDangerLevel;

        [BoxGroup("Dev")] [ShowInInspector] private string Dev_WarningSensorStatus =>
            warningSensor.ToString();

        [BoxGroup("Dev")] [ShowInInspector] private string Dev_CriticalSensorStatus =>
            _criticalSensor == null ? "Not Initialized." : _criticalSensor.ToString();

        [BoxGroup("Dev")] [ShowInInspector] private string Dev_GameOverTimerStatus =>
            _gameOverTimer == null ? "Not Initialized." : _gameOverTimer.ToString();

        [BoxGroup("Dev")] [Button(Name = "Force GameOver")]
        private void Dev_ForceGameOver()
        {
            GameOver();
        }
    }
}

#endif