namespace TBM.MaouSuika.Core.UI
{
    public class MessageBoxResult
    {
        public readonly MessageBoxButtonAction Action;
        public readonly object Data;

        public MessageBoxResult(MessageBoxButtonAction action, object data = null)
        {
            Action = action;
            Data = data;
        }

        public override string ToString()
        {
            return $"Tag[{Action}], Data[{Data}]";
        }
    }
}