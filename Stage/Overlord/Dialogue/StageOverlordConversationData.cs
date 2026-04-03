namespace SOSG.System.Dialogue
{
    public class StageOverlordConversationData : ConversationData
    {
        public StageOverlordLineType LineType;
        public bool IsConjunctionAdded;

        public StageOverlordConversationData(string text, StageOverlordLineType lineType) : base(text)
        {
            Text = text;
            LineType = lineType;
            IsConjunctionAdded = false;
        }
    }
}