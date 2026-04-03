using UnityEngine.Localization;

namespace TBM.MaouSuika.Core.UI
{
    public class MessageBoxButtonOption
    {
        public LocalizedString Content { get; private set; } = new();
        public MessageBoxButtonAction Action { get; private set; } = new();
        public bool IsDefault { get; private set; } = false;

        private MessageBoxButtonOption() { }


        #region Builder

        public static MessageBoxButtonOption Create()
        {
            return new MessageBoxButtonOption();
        }

        public MessageBoxButtonOption SetContent(LocalizedString content)
        {
            Content = content;
            return this;
        }

        public MessageBoxButtonOption SetAction(MessageBoxButtonAction action)
        {
            Action = action;
            return this;
        }

        public MessageBoxButtonOption SetDefault(bool isDefault = true)
        {
            IsDefault = isDefault;
            return this;
        }

        #endregion

        public override string ToString()
        {
            return $"Content Key[{Content.TableEntryReference.Key}], Tag[{Action}]";
        }
    }
}