using System.Threading;
using Coffee.UIExtensions;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using SOSG.Stage;
using UnityEngine;
using UnityEngine.UI;

namespace SOSG.Skill
{
    public class SkillUI : MonoBehaviour
    {
        [SerializeField] private StageManagerSO stageManagerSO;

        [SerializeField] private SkillSystemSO skillSystemSO;
        [SerializeField] private Sprite[] iconSpriteArr;

        [SerializeField] private SkillRewardedAdIconUI rewardedAdIconUI; 

        [SerializeField] private RectTransform rt;
        [SerializeField] private Button skillBtn;
        [SerializeField] private Image gaugeImage;
        [SerializeField] private UIParticle particle;

        private int _curGaugeIdx;

        private bool _isInteractable;

        private float _aniSpeed;

        private CancellationTokenSource _destroyCts;


        private void Awake()
        {
            _aniSpeed = rt.sizeDelta.x / 0.25f;

            _destroyCts = new CancellationTokenSource();

            particle.StopEmission();
            InitializeCallbacks(true);
            SetInteractable(false);
        }

        private void OnDestroy()
        {
            InitializeCallbacks(false);
            _destroyCts.CancelAndDispose();
        }

        private void InitializeCallbacks(bool value)
        {
            if (value)
            {
                skillSystemSO.OnSkillActivated += OnSkillActivated;
                skillSystemSO.OnSkillDeactivated += OnSkillDeactivated;
                skillSystemSO.OnSkillGaugeChanged += OnSkillGaugeChanged;
                skillSystemSO.OnAdIconSetEnableRequested = rewardedAdIconUI.SetEnable;
                stageManagerSO.ActionOnStageStarted += OnStageStarted;
                stageManagerSO.ActionOnStageEnded += OnStageEnded;
            }
            else
            {
                skillSystemSO.OnSkillActivated -= OnSkillActivated;
                skillSystemSO.OnSkillDeactivated -= OnSkillDeactivated;
                skillSystemSO.OnSkillGaugeChanged -= OnSkillGaugeChanged;
                skillSystemSO.OnAdIconSetEnableRequested = null;
                stageManagerSO.ActionOnStageStarted -= OnStageStarted;
                stageManagerSO.ActionOnStageEnded -= OnStageEnded;
            }
        }

        private void OnStageStarted()
        {
            SetInteractable(true);
        }

        private void OnStageEnded()
        {
            SetInteractable(false);
        }

        public void OnSkillBtnClicked() => skillSystemSO.OnSkillUiClicked();

        private void OnSkillActivated() => Hide();
        private void OnSkillDeactivated() => ShowAsync().Forget();

        private void OnSkillGaugeChanged(float gauge)
        {
            // gauge에 따라 보여줄 sprite 결정
            gauge = Mathf.Clamp(gauge, 0f, 1f);
            var value = (iconSpriteArr.Length - 1) * gauge;
            var index = Mathf.FloorToInt(value);
            if (_curGaugeIdx == index)
            {
                return;
            }

            _curGaugeIdx = index;
            gaugeImage.sprite = iconSpriteArr[index];

            if (gauge >= 1f)
            {
                particle.StartEmission();
            }
            else
            {
                particle.StopEmission();
            }
        }

        private void SetInteractable(bool value)
        {
            // todo input 디바이스 별 조작
            skillBtn.interactable = value;
        }


        private async UniTaskVoid ShowAsync()
        {
            await rt.DOAnchorPosX(0f, _aniSpeed)
                .SetSpeedBased(true).SetUpdate(true).SetEase(Ease.OutSine).Play()
                .AwaitForComplete(TweenCancelBehaviour.KillAndCancelAwait, _destroyCts.Token);
            SetInteractable(true);
        }

        private void Hide()
        {
            SetInteractable(false);
            rt.DOAnchorPosX(-rt.sizeDelta.x, _aniSpeed)
                .SetSpeedBased(true).SetUpdate(true).SetEase(Ease.OutSine).Play()
                .WithCancellation(_destroyCts.Token);
        }
    }
}