using System;
using Cysharp.Threading.Tasks;
using SOSG.System;
using UnityEngine;

namespace SOSG.System.Dialogue
{
    public static class DialogueHelper
    {
        private static ConversationController conversationController;
        private static ConversationController ConversationController
        {
            get
            {
                if (ReferenceEquals(conversationController, null))
                {
                    conversationController = GameManager.Instance.DialogueManager.ConversationController;
                }

                return conversationController;
            }
        }

        private static DialogueManager manager;
        private static DialogueManager Manager
        {
            get
            {
                if (ReferenceEquals(manager, null))
                {
                    manager = GameManager.Instance.DialogueManager;
                }

                return manager;
            }
        }


        public static event Action<string[]> ConversationEventRaised
        {
            add => conversationController.ConversationEventRaised += value;
            remove => conversationController.ConversationEventRaised -= value;
        }
        public static event Action<long> ConversationFinished
        {
            add => conversationController.ConversationPrinted += value;
            remove => conversationController.ConversationPrinted -= value;
        }

        public static event Action ConversationBoxShowRequested;
        public static event Action ConversationBoxHideRequested;
        
        public static event Action ConversationPauseRequested;
        public static event Action ConversationUnPauseRequested;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void InitializeOnSubsystemRegistration()
        {
            manager = null;
            conversationController = null;
        }
        
        public static void ShowConversation() => ConversationBoxShowRequested?.Invoke();
        
        public static void HideConversation() => ConversationBoxHideRequested?.Invoke();

        public static void Pause() => ConversationPauseRequested?.Invoke();

        public static void UnPause() => ConversationUnPauseRequested?.Invoke();

        public static void SetPortraitActive(bool value) => conversationController.SetPortrait(value);

        public static void PrintConversation(string line)
        {
            var lineData = new ConversationData(line);
            ConversationController.Print(lineData);
        }

        public static void PrintConversation(ConversationData data)
        {
            ConversationController.Print(data);
        }

        public static async UniTask<int> ShowChoiceAsync(ChoiceData choiceData)
        {
            return await Manager.ShowChoice(choiceData);
        }
    }
}