namespace SOSG.System.Dialogue
{
    public class ChoiceData
    {
        public ConversationData question;
        public string[] answerArr;

        public ChoiceData(ConversationData question, string[] answerArr)
        {
            this.question = question;
            this.answerArr = answerArr;
        }
    }
}