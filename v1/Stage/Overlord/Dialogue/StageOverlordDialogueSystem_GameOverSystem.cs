using SOSG.Stage.GameOver;
using SOSG.System.Localization;
using UnityEngine;

namespace SOSG.System.Dialogue
{
    public partial class StageOverlordDialogueSystem
    {
        [Header("GameOver System")] [SerializeField]
        private GameOverSystemStateSO gameOverSystemStateSO;


        private void AwakeGameOver()
        {
            gameOverSystemStateSO.ActionOnStateChanged += OnGameOverSystemStateChanged;
        }

        private void OnDestroyGameOver()
        {
            gameOverSystemStateSO.ActionOnStateChanged -= OnGameOverSystemStateChanged;
        }

        private void OnGameOverSystemStateChanged(GameOverSystemState gameOverSystemState)
        {
            const string safeToWarningLineKey = "safe-to-warning";
            const string countdownStartedLineKey = "countdown-started";
            const string countDownCanceledLineKey = "countdown-canceled";
            const string warningToSafeLineKey = "warning-to-safe";
            const int randomLineCount = 5;

            if (gameOverSystemStateSO.PrevState is GameOverSystemState.Safe &&
                gameOverSystemState is GameOverSystemState.Warning)
            {
                RequestRandomLine(LocalizationTableName.Stage_System, safeToWarningLineKey, randomLineCount,
                    StageOverlordLineType.GameOver);
            }
            else if (gameOverSystemState is GameOverSystemState.Countdown)
            {
                RequestRandomLine(LocalizationTableName.Stage_System, countdownStartedLineKey, randomLineCount,
                    StageOverlordLineType.GameOver);
            }
            else if (gameOverSystemState is GameOverSystemState.GameOver)
            {
                return;
            }
            else if (gameOverSystemStateSO.PrevState is GameOverSystemState.Countdown)
            {
                RequestRandomLine(LocalizationTableName.Stage_System, countDownCanceledLineKey, randomLineCount,
                    StageOverlordLineType.GameOver);
            }
            else if (gameOverSystemState is GameOverSystemState.Safe)
            {
                RequestRandomLine(LocalizationTableName.Stage_System, warningToSafeLineKey, randomLineCount,
                    StageOverlordLineType.GameOver);
            }
            else
            {
                Debug.LogWarning($"{GetType()} - 다른 경우의 수가 존재하나봄.");
            }
        }
    }
}