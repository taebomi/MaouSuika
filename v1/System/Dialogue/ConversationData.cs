namespace SOSG.System.Dialogue
{
    public class ConversationData
    {
        public long ID;
        public string Text;
        public bool WillDisappear = true;

        public ConversationData(long id, string text, bool willDisappear = true)
        {
            ID = id;
            Text = text;
            WillDisappear = willDisappear;
        }

        public ConversationData(string text)
        {
            Text = text;
        }
    }
}