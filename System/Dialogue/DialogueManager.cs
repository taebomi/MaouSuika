using System;
using Cysharp.Threading.Tasks;
using SOSG.System.Scene;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SOSG.System.Dialogue
{
    public class DialogueManager : MonoBehaviour
    {
        [field: SerializeField] public ConversationController ConversationController { get; private set; }
        [field: SerializeField] public ChoiceBox ChoiceBox { get; private set; }

        private void OnEnable()
        {
            DialogueHelper.ConversationBoxShowRequested += ConversationController.Show;
            DialogueHelper.ConversationBoxHideRequested += ConversationController.Hide;
        }

        private void OnDisable()
        {
            DialogueHelper.ConversationBoxShowRequested -= ConversationController.Show;
            DialogueHelper.ConversationBoxHideRequested -= ConversationController.Hide;
        }


        private void Start()
        {
            if (SceneContext.CurSceneName == SceneName.Init)
            {
                ConversationController.SetPortrait(false);
            }
        }

        public async UniTask<int> ShowChoice(ChoiceData choiceData)
        {
            var lastSelectedGameObject = EventSystem.current.currentSelectedGameObject;
            EventSystem.current.SetSelectedGameObject(null);
            ConversationController.Print(choiceData.question);
            ChoiceBox.ShowBackgroundOnly();
            await ChoiceBox.ShowAsync(choiceData);
            var selectedIdx = await ChoiceBox.WaitForChoiceAsync();
            ChoiceBox.Hide();
            ConversationController.DisappearText();
            EventSystem.current.SetSelectedGameObject(lastSelectedGameObject);
            return selectedIdx;
        }
    }
}