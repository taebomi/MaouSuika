using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using SOSG.System.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SOSG.System.Dialogue
{
    public class ChoiceBox : BaseUI, IModalUI
    {
        private const float ButtonYPosMultiplier = 80f;
        private const float ButtonYInterval = 160f;

        [SerializeField] private ConversationController conversationController;

        [SerializeField] private RectTransform choiceBtnContainerRt;
        [SerializeField] private ChoiceButton[] choiceButtons;

        protected override void AwakeAfter()
        {
            Canvas.enabled = false;
        }

        public void OnOverlayClicked()
        {
            conversationController.Skip();
        }

        public void ShowBackgroundOnly()
        {
            UIHelper.ShowModalOverlay(this);
        }

        public async UniTask ShowAsync(ChoiceData choiceData)
        {
            SetUpButtons(choiceData.answerArr);

            UIHelper.AddSortingOrder(this);
            Canvas.enabled = true;

            await choiceBtnContainerRt.SOSGUIPopUp().Play();

            EventSystem.current.SetSelectedGameObject(choiceButtons[0].gameObject);
        }

        public async UniTask<int> WaitForChoiceAsync()
        {
            var choiceCt = new CancellationTokenSource();
            var cts = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken, choiceCt.Token);

            var clickedIdx = await UniTask.WhenAny(choiceButtons.Select(Selector));
            choiceCt.CancelAndDispose();

            return clickedIdx;

            UniTask Selector(ChoiceButton choiceButton) => choiceButton.OnClickAsync(cts.Token);
        }

        public void Hide()
        {
            Canvas.enabled = false;
            UIHelper.HideUI(this);
        }


        private void SetUpButtons(IReadOnlyList<string> choices)
        {
            var startYPos = (choices.Count - 1) * ButtonYPosMultiplier;

            for (var i = 0; i < choices.Count; i++)
            {
                choiceButtons[i].SetText(choices[i]);
                choiceButtons[i].SetPosition(new Vector2(0f, startYPos - i * ButtonYInterval));
                choiceButtons[i].SetActive(true);
            }

            for (var i = choices.Count; i < choiceButtons.Length; i++)
            {
                choiceButtons[i].SetActive(false);
            }
        }
    }
}