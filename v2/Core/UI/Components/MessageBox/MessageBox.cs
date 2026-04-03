using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TBM.Extensions;
using UnityEngine;
using UnityEngine.Localization.Components;

namespace TBM.MaouSuika.Core.UI
{
    public class MessageBox : ModalUI
    {
        private const int BUTTON_COUNT = 4;
        
        // 변경 시 함수 수정 필요
        // 여유 있을 때, 동적 생성으로 변경 
        [SerializeField] private LocalizeStringEvent message;
        [SerializeField] private GameObject buttonRow1;
        [SerializeField] private MessageBoxButton button1;
        [SerializeField] private MessageBoxButton button2;
        [SerializeField] private GameObject buttonRow2;
        [SerializeField] private MessageBoxButton button3;
        [SerializeField] private MessageBoxButton button4;

        private UniTaskCompletionSource _buttonClickTcs;

        public void Initialize(MessageBoxRequest request)
        {
            message.StringReference = request.Message;
            InitializeButtons(request.ButtonOptions);
        }
        
        /// <summary>
        /// 버튼 위치 및 데이터 초기화 
        /// </summary>
        private void InitializeButtons(List<MessageBoxButtonOption> buttonOptions)
        {
            // 유효성 체크
            if (buttonOptions.Count > BUTTON_COUNT)
            {
                Logger.Error($"Too many requested button[{buttonOptions.Count}], " +
                                $"Removed requested button[{BUTTON_COUNT} ~ {buttonOptions.Count - 1}]");
                buttonOptions.RemoveRange(BUTTON_COUNT, buttonOptions.Count - BUTTON_COUNT);
            }
            else if (buttonOptions.Count == 0)
            {
                Logger.Error($"No requested button");
                buttonRow1.SetActive(false);
                buttonRow2.SetActive(false);
                return;
            }

            // 버튼 초기화
            switch (buttonOptions.Count)
            {
                case 1:
                    button1.Initialize(buttonOptions[0]);
                    button2.SetActive(false);
                    buttonRow1.SetActive(true);
                    buttonRow2.SetActive(false);
                    break;
                case 2:
                    button1.Initialize(buttonOptions[0]);
                    button2.Initialize(buttonOptions[1]);
                    buttonRow1.SetActive(true);
                    buttonRow2.SetActive(false);
                    break;

                case 3:
                    button1.Initialize(buttonOptions[0]);
                    button2.SetActive(false);
                    buttonRow1.SetActive(true);
                    
                    button3.Initialize(buttonOptions[1]);
                    button4.Initialize(buttonOptions[2]);
                    buttonRow2.SetActive(true);
                    break;

                case 4:
                    button1.Initialize(buttonOptions[0]);
                    button2.Initialize(buttonOptions[1]);
                    buttonRow1.SetActive(true);
                    button3.Initialize(buttonOptions[2]);
                    button4.Initialize(buttonOptions[3]);
                    buttonRow2.SetActive(true);
                    break;
            }
        }
        
        
        public UniTask<MessageBoxResult> WaitForResultAsync()
        {
            var tcs = new UniTaskCompletionSource<MessageBoxResult>();

            var buttons = new[] { button1, button2, button3, button4 };
            foreach (var btn in buttons)
            {
                btn.Clicked += OnButtonClicked;
            }

            return tcs.Task;

            void OnButtonClicked(MessageBoxButton button)
            {
                foreach (var btn in buttons)
                {
                    btn.Clicked -= OnButtonClicked;
                }

                var result = new MessageBoxResult(button.Action);
                tcs.TrySetResult(result);
            }
        }
    }
}