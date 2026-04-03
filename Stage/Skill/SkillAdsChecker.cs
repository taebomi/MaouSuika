using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using SOSG.Skill;
using UnityEngine;

namespace SOSG.Ads
{
    public class SkillAdsChecker : MonoBehaviour
    {
        [SerializeField] private SkillSystemSO skillSystemSO;
        [SerializeField] private AdsManagerSO adsManagerSO;
        [SerializeField] private StageAdsSO stageAdsSO;

        public bool IsAdLoaded { get; private set; }
        public bool HasRewarded { get; private set; }

        private bool _adShowedFlag;

        private CancellationTokenSource _reloadAdCts;
        private CancellationTokenSource _destroyCts;

        private const string RewardedAdId = AdUnitId.SkillChargeRewardedAdUnitId;

        private void Awake()
        {
            _destroyCts = new CancellationTokenSource();
        }

        private void OnDestroy()
        {
            _reloadAdCts?.CancelAndDispose();
            _destroyCts?.CancelAndDispose();
        }

        public void Initialize()
        {
            HasRewarded = false;

            if (adsManagerSO.CanShowRewardedAd(RewardedAdId))
            {
                OnAdLoaded();
            }
            else
            {
                LoadAd();
            }
        }

        public void ShowAd(Action<bool> succeedCallback)
        {
            adsManagerSO.ShowRewardedAd(RewardedAdId, succeed => OnAdShowed(succeed, succeedCallback));
        }

        private void OnAdShowed(bool succeed, Action<bool> succeedCallback)
        {
            succeedCallback?.Invoke(succeed);
            OnAdShowed(succeed);
        }

        public void SetEnable(bool value)
        {
            if (value)
            {
                if (IsAdLoaded && HasRewarded is false)
                {
                    skillSystemSO.NotifyRewardedAdReady(true);
                }
            }
            else
            {
                skillSystemSO.NotifyRewardedAdReady(false);
            }
        }

        private void LoadAd()
        {
            if (!gameObject)
            {
                return;
            }
            IsAdLoaded = false;
            adsManagerSO.LoadRewardedAd(RewardedAdId, OnAdLoadFinished);
        }

        private void OnAdLoadFinished(bool succeed)
        {
            if (succeed)
            {
                OnAdLoaded();
            }
            else
            {
                ReloadAd().Forget();
            }
        }

        private void OnAdLoaded()
        {
            IsAdLoaded = true;
            if (HasRewarded is false)
            {
                skillSystemSO.NotifyRewardedAdReady(true);
            }
        }

        private void OnAdShowed(bool succeed)
        {
            if (succeed)
            {
                HasRewarded = true;
                skillSystemSO.NotifyRewardedAdReady(false);
                stageAdsSO.NotifyRewarded();
            }

            LoadAd();
        }

        private async UniTaskVoid ReloadAd()
        {
            _reloadAdCts?.CancelAndDispose();
            _reloadAdCts = new CancellationTokenSource();
            const double retryDelay = 15;
            await UniTask.Delay(TimeSpan.FromSeconds(retryDelay), cancellationToken: _reloadAdCts.Token);
            LoadAd();
        }
    }
}