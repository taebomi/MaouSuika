using System.Collections.Generic;
using TBM.MaouSuika.Core.Localization;
using UnityEngine.Localization;

namespace TBM.MaouSuika.Core.UI
{
    public class MessageBoxRequest
    {
        public LocalizedString Message = new();
        public List<MessageBoxButtonOption> ButtonOptions = new();

        private MessageBoxRequest() { }

        #region Builder Pattern

        public static MessageBoxRequest Create()
        {
            return new MessageBoxRequest();
        }

        public MessageBoxRequest SetMessage(string message)
        {
            var localizedString = LocalizedStrings.System.ArguOnly;
            localizedString.Arguments = new object[] { message };
            Message = localizedString;
            return this;
        }

        public MessageBoxRequest SetMessage(LocalizedString message)
        {
            Message = message;
            return this;
        }

        public MessageBoxRequest AddButton(MessageBoxButtonOption option)
        {
            ButtonOptions.Add(option);
            return this;
        }
        

        #endregion
    }
}