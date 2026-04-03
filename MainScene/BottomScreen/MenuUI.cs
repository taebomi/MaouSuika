using System;
using Cysharp.Threading.Tasks;
using SOSG.System.Localization;
using SOSG.System.Scene;
using SOSG.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using Button = UnityEngine.UI.Button;
using Random = UnityEngine.Random;

namespace SOSG.MainScene
{
    public class MenuUI : MonoBehaviour
    {
        [SerializeField] private IntEventSO timingEventSO;

        [SerializeField] private Canvas canvas;
        [SerializeField] private SettingUI settingUI;
        [SerializeField] private CashShopView cashShopView;
        [SerializeField] private CreditsView creditsView;

        [SerializeField] private Button firstSelectedButton;

        private TBMStringTable _tbmStringTable;

        private void Awake()
        {
            canvas.enabled = false;
            SceneSetUpHelper.AddTask(SetUpAsync);
        }

        private void OnEnable()
        {
            SceneSetUpHelper.Completed += OnSceneSetUp;
            timingEventSO.OnEventRaised += OnBgmTimingEventRaised;
        }

        private void OnDisable()
        {
            timingEventSO.OnEventRaised -= OnBgmTimingEventRaised;
            SceneSetUpHelper.Completed -= OnSceneSetUp;
        }

        private async UniTask SetUpAsync()
        {
            _tbmStringTable = GetComponent<TBMStringTable>();
            await _tbmStringTable.SetUpAsync(LocalizationTableName.MainScene);
        }

        private void OnSceneSetUp()
        {
            if (MainSceneManager.SkipIntro)
            {
                ShowMenuUI();
            }
        }

        private void OnBgmTimingEventRaised(int timing)
        {
            switch (timing)
            {
                case 10:
                    ShowMenuUI();
                    break;
            }
        }

        private void ShowMenuUI()
        {
            canvas.enabled = true;
            EventSystem.current.SetSelectedGameObject(firstSelectedButton.gameObject);
        }

        public void OnStartBtnClicked()
        {
            _tbmStringTable.PrintConversation($"endless_mode_start_{Random.Range(1, 6)}");
            SceneLoadHelper.LoadScene(SceneName.EndlessMode);
        }

        public void OnMultiplayerBtnClicked()
        {
            _tbmStringTable.PrintConversation($"multiplayer_mode_start");
            SceneLoadHelper.LoadScene(SceneName.SplitScreenModeGame, TransitionType.TopAndBottom, TransitionType.All);
        }

        public void OnShopBtnClicked()
        {
            cashShopView.ShowAsync().Forget();
        }

        public void OnCustomizeBtnClicked()
        {
            _tbmStringTable.PrintConversation("customization_clicked");
            SceneLoadHelper.LoadScene(SceneName.Customization);
        }

        public void OnSettingBtnClicked()
        {
            settingUI.Show();
        }

        public void OnCreditsBtnClicked()
        {
            creditsView.ShowAsync().Forget();
        }

        public void OnQuitBtnClicked()
        {
            AskQuit().Forget();
        }

        private async UniTaskVoid AskQuit()
        {
            var result = await _tbmStringTable.ShowChoiceAsync("quit_question", "quit_answer");
            if (result == 0)
            {
                _tbmStringTable.PrintConversation("quit_confirm");
                await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: destroyCancellationToken);
                Application.Quit();
            }
            else
            {
                _tbmStringTable.PrintConversation("quit_cancel");
            }
        }
    }
}