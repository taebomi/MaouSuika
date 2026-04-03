using SOSG.System.Localization;

namespace SOSG.System.Dialogue
{
    public partial class StageOverlordDialogueSystem
    {
        private void OnStageStarted()
        {
            const string stageStartLineKey = "stage-start";
            const int lineCount = 5;
            RequestRandomLine(LocalizationTableName.Stage_System, stageStartLineKey, lineCount, StageOverlordLineType.GameOver);
            // dialogueHelper.OnStageStarted();
        }

        private void OnStageEnded()
        {
            const string stageEndLineKey = "stage-end";
            const int lineCount = 5;
            RequestRandomLine(LocalizationTableName.Stage_System, stageEndLineKey, lineCount, StageOverlordLineType.GameOver);
            // dialogueHelper.OnStageEnded();
        }
    }
}