using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using SOSG.System.Dialogue;
using SOSG.Scene;
using SOSG.Skill;
using SOSG.System;
using SOSG.System.Input;
using SOSG.System.Localization;
using SOSG.System.Scene;
using SOSG.System.UI;
using SOSG.UI;
using UnityEngine;
using UnityEngine.Localization.Tables;
using UnityEngine.Serialization;

namespace SOSG.Stage.UI
{
    public class Pause : MonoBehaviour, IModalUI
    {
        [SerializeField] private GameInputSO gameInputSO;
        [SerializeField] private StageManagerSO stageManagerSO;
        [SerializeField] private SkillSystemSO skillSystemSO;

        [SerializeField] private SettingUI settingUI;

        [SerializeField] private PauseActivateBtn pauseActivateBtn;
        
        [field:SerializeField] public Canvas Canvas { get; private set; }
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private RectTransform mainButtonContainerRt;
        [SerializeField] private CanvasGroup mainButtonContainerCanvasGroup;

        // private LocalizationHelper _localizationHelper;
        private TempDialogueHelper _tempDialogueHelper;


        private const float AniDuration = 0.25f;

        public const float BtnPosY = -350f;
        public const float BtnMoveY = 50f;
        public const float BtnAniDuration = 0.15f;
        public const Ease BtnShowEase = Ease.OutSine;
        public const Ease BtnHideEase = Ease.InSine;

        private void Awake()
        {
            stageManagerSO.ActionOnStageStarted += ShowBtn;
            stageManagerSO.ActionOnStageEnded += HideBtn;
            skillSystemSO.OnSkillActivated += HideBtn;
            skillSystemSO.OnSkillDeactivated += ShowBtn;


            canvasGroup.alpha = 1f;
            gameObject.SetActive(false);
            SceneSetUpHelper.AddTask(SetUpAsync);
        }

        private void OnDestroy()
        {
            stageManagerSO.ActionOnStageStarted -= ShowBtn;
            stageManagerSO.ActionOnStageEnded -= HideBtn;
            skillSystemSO.OnSkillActivated -= HideBtn;
            skillSystemSO.OnSkillDeactivated -= ShowBtn;
        }

        private async UniTask SetUpAsync()
        {
            // await _localizationHelper.SetUpAsync(LocalizationTableName.Stage_Pause);
        }

        private void ShowBtn()
        {
            pauseActivateBtn.Show().Forget();
        }

        private void HideBtn()
        {
            pauseActivateBtn.Hide().Forget();
        }

        public void Show() => ShowAsync().Forget();

        public async UniTaskVoid ShowAsync()
        {
            gameObject.SetActive(true);
            // modalUI.Show();
            gameInputSO.DisableStageControl();
            canvasGroup.blocksRaycasts = false;
            CanInteract = false;
            _tempDialogueHelper.SetListener(true);
            _tempDialogueHelper.RequestLine("activate");
            TBMTimeScale.Set(this, 0f);
            await transform.DOScaleX(1f, AniDuration).From(0f).SetEase(Ease.OutBack).SetUpdate(true).Play()
                .AwaitForComplete(TweenCancelBehaviour.KillAndCancelAwait, destroyCancellationToken);
            canvasGroup.blocksRaycasts = true;
            CanInteract = true;
        }

        private void Hide()
        {
            gameObject.SetActive(false);
            TBMTimeScale.Unset(this);
            _tempDialogueHelper.RequestLine("deactivate");
            _tempDialogueHelper.SetListener(false);
            // modalUI.Hide();
            gameInputSO.EnableStageControl();
        }

        #region Main Menu

        public void OnHomeBtnClicked() => AskHome().Forget();

        public void OnSettingBtnClicked() => OnSettingBtnClickedAsync().Forget();

        private async UniTaskVoid OnSettingBtnClickedAsync()
        {
            CanInteract = false;
            canvasGroup.blocksRaycasts = false;
            settingUI.Show();
            await DOTween.Sequence().SetUpdate(true).SetEase(BtnHideEase)
                .Append(mainButtonContainerRt.DOAnchorPosY(BtnPosY + BtnMoveY, BtnAniDuration)
                    .From(new Vector2(0f, BtnPosY)))
                .Join(mainButtonContainerCanvasGroup.DOFade(0f, BtnAniDuration).From(1f)).Play()
                .WithCancellation(destroyCancellationToken);
            canvasGroup.blocksRaycasts = true;
            CanInteract = true;
            gameObject.SetActive(false);
        }

        public void OnRestartBtnClicked() => AskRestart().Forget();

        private async UniTaskVoid AskHome()
        {
            _tempDialogueHelper.RequestLine("ask-home");
            var answer = await _tempDialogueHelper.RequestChoiceAsync("choice-home");
            if (answer is 0)
            {
                SceneLoadHelper.LoadScene(SceneName.Main);
                _tempDialogueHelper.RequestLine("home");
            }
            else
            {
                _tempDialogueHelper.RequestLine("cancel-home");
            }
        }

        private async UniTaskVoid AskRestart()
        {
            _tempDialogueHelper.RequestLine("ask-restart");
            var answer = await _tempDialogueHelper.RequestChoiceAsync("choice-restart");
            if (answer is 0)
            {
                SceneLoadHelper.LoadScene(SceneName.EndlessMode);
                _tempDialogueHelper.RequestLine("restart");
            }
            else
            {
                _tempDialogueHelper.RequestLine("cancel-restart");
            }
        }

        #endregion


        #region Setting Menu

        public async UniTaskVoid ReturnPauseMenu()
        {
            gameObject.SetActive(true);
            canvasGroup.blocksRaycasts = false;
            CanInteract = false;
            await DOTween.Sequence().SetUpdate(true).SetEase(BtnHideEase)
                .Append(mainButtonContainerRt.DOAnchorPosY(BtnPosY, BtnAniDuration)
                    .From(new Vector2(0f, BtnPosY - BtnMoveY)))
                .Join(mainButtonContainerCanvasGroup.DOFade(1f, BtnAniDuration).From(0f)).Play()
                .WithCancellation(destroyCancellationToken);
            CanInteract = true;
            canvasGroup.blocksRaycasts = true;
        }

        #endregion

        public bool CanInteract { get; private set; }

        public void OnOverlayClicked()
        {
            //todo   
        }
    }
}