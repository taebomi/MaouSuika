using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using SOSG.Ads;
using SOSG.System.Dialogue;
using SOSG.Stage;
using SOSG.System;
using SOSG.System.Input;
using SOSG.System.Localization;
using SOSG.System.Scene;
using UnityEngine;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace SOSG.Skill
{
    public class SkillManager : MonoBehaviour
    {
        [field: SerializeField] public GameInputSO InputSO { get; private set; }

        [SerializeField] private StageManagerSO stageManagerSO;
        [SerializeField] private SkillSystemSO skillSystemSO;

        [field: SerializeField] public GashaponColliderDictVarSO GashaponCollDictVarSO { get; private set; }
        [field: SerializeField] public CameraVarSO TopScreenCameraVarSO { get; private set; }
        [field: SerializeField] public CameraVarSO BottomScreenCameraVarSO { get; private set; }

        [SerializeField] private IntEventSO scoreGetEventSO;

        [SerializeField] private SuikaSlash curSkill;
        [SerializeField] private SkillAdsChecker adsChecker;

        private int _curGauge; // 현재 게이지
        private State _curState;
        private bool _alreadyCharged; // 이미 스킬 충전 되어 있는지
        private bool _hasTriedActivateSkillWhenNotCharged; // 스킬 충전 안되어 있을 때 스킬 사용 시도한 적 있는지


        private const int SkillGauge = 1500;

        private enum State
        {
            NotCharged,
            Charged,
            Using,
        }


        private void Awake()
        {
            InitializeCallbacks(true);
            curSkill.Initialize(this);

            // SceneSetUpHelper.AddTask(SetUpAsync);
        }

        private void OnDestroy()
        {
            InitializeCallbacks(false);
        }

        private void InitializeCallbacks(bool value)
        {
            if (value)
            {
                skillSystemSO.OnSkillGaugeChargeRequested += OnSkillGaugeChargeRequested;
                skillSystemSO.OnSkillUIClicked += OnSkillUIClicked;
                scoreGetEventSO.OnEventRaised += OnScoreGet;
                stageManagerSO.ActionOnStageStarted += OnStageStart;
                stageManagerSO.ActionOnStageEnded += OnStageEnded;
            }
            else
            {
                skillSystemSO.OnSkillGaugeChargeRequested -= OnSkillGaugeChargeRequested;
                stageManagerSO.ActionOnStageStarted -= OnStageStart;
                skillSystemSO.OnSkillUIClicked -= OnSkillUIClicked;
                scoreGetEventSO.OnEventRaised -= OnScoreGet;
                stageManagerSO.ActionOnStageEnded -= OnStageEnded;
            }
        }

        // private async UniTask SetUpAsync()
        // {
        //     _localizationHelper = GetComponent<LocalizationHelper>();
        //     DialogueHelper = GetComponent<DialogueHelper>();
        //     await _localizationHelper.SetUpAsync(LocalizationTableName.Stage_Skill);
        //     adsChecker.Initialize();
        // }

        private void OnStageStart()
        {
            _alreadyCharged = false;
            _hasTriedActivateSkillWhenNotCharged = false;
            _curState = State.NotCharged;
            _curGauge = 0;
            skillSystemSO.NotifySkillGaugeChanged(0f);
        }

        private void OnStageEnded()
        {
            if (_curState is State.Using)
            {
                CancelSkill();
            }
        }

        private void OnScoreGet(int score)
        {
            _curGauge += score;
            _curGauge = Mathf.Clamp(_curGauge, 0, SkillGauge);
            CheckGauge();
        }

        private void OnSkillGaugeChargeRequested()
        {
            _curGauge = SkillGauge;
            CheckGauge();
        }

        private void CheckGauge()
        {
            if (_curGauge == SkillGauge)
            {
                if (_alreadyCharged is false)
                {
                    _alreadyCharged = true;
                    _curState = State.Charged;
                    skillSystemSO.NotifySkillGaugeChanged(1f);
                    adsChecker.SetEnable(false);
                    // TempDialogueHelper.RequestLine("skill-charged");
                }
            }
            else
            {
                _curState = State.NotCharged;
                skillSystemSO.NotifySkillGaugeChanged((float)_curGauge / SkillGauge);
            }
        }

        private void OnSkillUIClicked()
        {
            switch (_curState)
            {
                case State.NotCharged:
                    OnSkillUIClickedWhenNotCharged();
                    return;
                case State.Charged:
                    ActivateSkill();
                    break;
                case State.Using:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnSkillUIClickedWhenNotCharged()
        {
            // 보상 받은 경우 대사만 출력
            if (adsChecker.HasRewarded)
            {
                ShowNotChargedDialogue();
                return;
            }

            // 보상 받지 않은 경우
            if (adsChecker.IsAdLoaded is false) // 광고 로드 안된 경우 
            {
                ShowNotChargedDialogue();
            }
            else // 광고 로드 된 경우
            {
                AskRewardedAd().Forget();
            }
        }

        private void ShowNotChargedDialogue()
        {
            if (!_hasTriedActivateSkillWhenNotCharged)
            {
                // TempDialogueHelper.RequestLine("skill-not-charged-but-try-activate");
                _hasTriedActivateSkillWhenNotCharged = true;
            }
            else
            {
                // TempDialogueHelper.RequestLine("skill-not-charged-but-try-activate-again");
            }
        }

        private async UniTaskVoid AskRewardedAd()
        {
            TBMTimeScale.Set(this, 0f);

            // TempDialogueHelper.RequestLine("ask-show-skill-charge-rewarded-ad");
            // var answer = await TempDialogueHelper.RequestChoiceAsync("choice-show-skill-charge-rewarded-ad");
            // if (answer == 0)
            // {
            //     adsChecker.ShowAd(OnRewardedAdShowed);
            //     TBMTimeScale.Unset(this);
            // }
            // else
            // {
            //     TempDialogueHelper.RequestLine("show-skill-charge-rewarded-ad-canceled");
            //     TBMTimeScale.Unset(this);
            // }
        }

        private void OnRewardedAdShowed(bool succeed)
        {
            if (succeed is false)
            {
                // TempDialogueHelper.RequestLine("skill-charge-rewarded-ad-failed");
                return;
            }

            _alreadyCharged = true;
            _curGauge = SkillGauge;
            _curState = State.Charged;
            skillSystemSO.NotifySkillGaugeChanged(1f);
            // TempDialogueHelper.RequestLine("skill-charge-rewarded-ad-succeed");
        }

        private void ActivateSkill()
        {
            TBMTimeScale.Set(this, 0.2f);
            _curState = State.Using;
            InputSO.DisableStageControl();
            skillSystemSO.NotifySkillActivated();
            // TempDialogueHelper.SetListener(true);
            // TempDialogueHelper.RequestLine($"{curSkill.skillNameDialoguePrefix}-activated");
            CheckTouchFinished().Forget();
        }

        private async UniTaskVoid CheckTouchFinished()
        {
            await UniTask.WaitUntil(() => Touch.activeTouches.Count == 0, cancellationToken: destroyCancellationToken);
            curSkill.Activate();
        }

        private void CancelSkill()
        {
            curSkill.Cancel();
        }


        public void OnSkillUseFinished()
        {
            _curGauge = 0;
            skillSystemSO.NotifySkillGaugeChanged(0f);
            _curState = State.NotCharged;
            _alreadyCharged = false;
            _hasTriedActivateSkillWhenNotCharged = false;
            adsChecker.SetEnable(true);
            OnSkillDeactivated();
        }

        public void OnSkillCanceled()
        {
            _curState = State.Charged;
            OnSkillDeactivated();
            if (stageManagerSO.state is StageManagerSO.State.Playing)
            {
                // TempDialogueHelper.RequestLine($"{curSkill.skillNameDialoguePrefix}-canceled");
            }
        }

        private void OnSkillDeactivated()
        {
            // TempDialogueHelper.SetListener(false);
            InputSO.EnableStageControl();
            TBMTimeScale.Unset(this);
            skillSystemSO.NotifySkillDeactivated();
        }
    }
}