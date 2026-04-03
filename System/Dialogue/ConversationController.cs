using System;
using System.Threading;
using System.Xml;
using Cysharp.Threading.Tasks;
using Febucci.UI;
using Febucci.UI.Core.Parsing;
using FMODUnity;
using SOSG.System;
using SOSG.System.Audio;
using TaeBoMi;
using TMPro;
using UnityEngine;

namespace SOSG.System.Dialogue
{
    public class ConversationController : MonoBehaviour
    {
        [SerializeField] private ConversationTextBox textBox;
        [SerializeField] private LineBoxPortrait portrait;
        [SerializeField] private ConversationSfxController sfxController;

        private ConversationData _curConversationData;

        private EventReference _curToneSfxRef;

        public event Action<string[]> ConversationEventRaised;
        public event Action<long> ConversationPrinted;

        #region Callback

        public void OnTextShowed() // 출력 완료 시
        {
            portrait.CloseMouth();

            if (_curConversationData.WillDisappear)
            {
                textBox.StartWaiting().Forget();
            }
            else
            {
                textBox.OnFinished();
                OnConversationPrinted();
            }
        }

        public void OnConversationPrinted() // 사라지지 않는 대화 출력 완료 or 대기 완료 시
        {
            ConversationPrinted?.Invoke(_curConversationData.ID);
        }

        public void OnTextDisappeared() // 사라짐 완료 시
        {
            portrait.SetEmotion(OverlordEmotion.Normal);
            portrait.CloseMouth();
            textBox.OnFinished();
        }

        public void OnCharacterVisible(char c)
        {
            if (c == ' ')
            {
                return;
            }

            portrait.ShowRandomSprite();
            sfxController.PlaySfx();
        }

        public void OnMessage(EventMarker eventMarker)
        {
            switch (eventMarker.name)
            {
                case "tone": // 마왕 목소리 톤 변경
                    sfxController.SetTone(eventMarker.parameters[0]);
                    break;
                case "event": // 대사 중 이벤트
                    ConversationEventRaised?.Invoke(eventMarker.parameters);
                    break;
                case "emotion":
                    portrait.SetEmotion(eventMarker.parameters[0]);
                    break;
            }
        }

        #endregion

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Print(ConversationData conversationData)
        {
            ResetSystem();

            _curConversationData = conversationData;
            textBox.Print(_curConversationData.Text);
        }

        public UniTask WaitForConversationFinished(long waitingID)
        {
            var tcs = new UniTaskCompletionSource();

            ConversationPrinted += OnConversationFinished;
            return tcs.Task;

            void OnConversationFinished(long id)
            {
                if (id == waitingID)
                {
                    tcs.TrySetResult();
                    ConversationPrinted -= OnConversationFinished;
                }
            }
        }

        public void Skip()
        {
            textBox.Skip();
        }

        public void DisappearText()
        {
            textBox.StartDisappear();
        }

        public void SetPortrait(bool value) => portrait.SetActive(value);

        private void ResetSystem()
        {
            sfxController.ResetSfx();
            portrait.ResetPortrait();
            textBox.ResetTextBox();
        }
    }
}