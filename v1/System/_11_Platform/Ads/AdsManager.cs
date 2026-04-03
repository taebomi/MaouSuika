using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using GoogleMobileAds.Api;
using SOSG.System;
using SOSG.Utility;
using TaeBoMi;
using UnityEngine;
using InterstitialAd = GoogleMobileAds.Api.InterstitialAd;

namespace SOSG.Ads
{
    public class AdsManager : MonoBehaviour
    {
        [SerializeField] private AdsManagerSO adsManagerSO;

        private Dictionary<string, RewardedAdData> _rewardedAdDataDict;
        private Dictionary<string, InterstitialAdData> _interstitialAdDataDict;

        public void SetUp()
        {
            MobileAds.Initialize(OnMobileAdsInitialized);

            _rewardedAdDataDict = new Dictionary<string, RewardedAdData>();
            _interstitialAdDataDict = new Dictionary<string, InterstitialAdData>();

            InitializeCallbacks(true);
        }

        public void TearDown()
        {
            InitializeCallbacks(false);
        }


        private void InitializeCallbacks(bool value)
        {
            if (value)
            {
                adsManagerSO.OnInterstitialAdLoadRequested = OnInterstitialAdLoadRequested;
                adsManagerSO.OnRewardedAdLoadRequested = OnRewardedAdLoadRequested;
                adsManagerSO.OnCanShowInterstitialAdRequested = CanShowInterstitialAd;
                adsManagerSO.OnCanShowRewardedAdRequested = CanShowRewardedAd;
                adsManagerSO.OnInterstitialAdShowRequested = ShowInterstitialAd;
                adsManagerSO.OnRewardedAdShowRequested = ShowRewardedAd;
            }
            else
            {
                adsManagerSO.OnInterstitialAdLoadRequested = null;
                adsManagerSO.OnRewardedAdLoadRequested = null;
                adsManagerSO.OnCanShowInterstitialAdRequested = null;
                adsManagerSO.OnCanShowRewardedAdRequested = null;
                adsManagerSO.OnInterstitialAdShowRequested = null;
                adsManagerSO.OnRewardedAdShowRequested = null;
            }
        }

        private void OnMobileAdsInitialized(InitializationStatus initializationStatus)
        {
            TBMUtility.Log($"# Ads Manager - Initialize Succeed");
            InitializeAdapter(initializationStatus);
        }

        private void InitializeAdapter(InitializationStatus initializationStatus)
        {
            var adapterStatusMap = initializationStatus.getAdapterStatusMap();
            foreach (var (className, adapterStatus) in adapterStatusMap)
            {
                switch (adapterStatus.InitializationState)
                {
                    case AdapterState.NotReady:
                        TBMUtility.Log($"# Ads Manager - Adapter Not Ready : {className}");
                        break;
                    case AdapterState.Ready:
                        TBMUtility.Log($"# Ads Manager - Adapter Initialized : {className}");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        #region Load Ad Moethos

        private void OnInterstitialAdLoadRequested(string id, Action<bool> succeedCallback)
        {
            TBMUtility.Log($"# Ads Manager - Interstitial Ad({id}) Load Requested");
            if (_interstitialAdDataDict.TryGetValue(id, out var data) is false)
            {
                data = new InterstitialAdData(id);
                _interstitialAdDataDict.Add(id, data);
            }

            switch (data.AdState)
            {
                case AdState.NotLoaded:
                case AdState.LoadFailed:
                    LoadInterstitialAd(data, succeedCallback);
                    break;
                case AdState.Loading:
                    TBMUtility.Log($"## Ad is Loading now");
                    return;
                case AdState.LoadSucceed:
                case AdState.CanShow:
                    TBMUtility.Log($"## Ad has Already Loaded");
                    succeedCallback?.Invoke(true);
                    return;
                case AdState.Showing:
                    TBMUtility.Log($"## Ad is Showing now");
                    return;
                case AdState.ShowFailed:
                case AdState.ShowSucceed:
                    TBMUtility.Log($"## Ad Already Showed");
                    LoadInterstitialAd(data, succeedCallback);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void LoadInterstitialAd(InterstitialAdData adData, Action<bool> resultCallback)
        {
            adData.AdState = AdState.Loading;
            CheckInterstitialAdLoadFinishedAsync(adData, resultCallback).Forget();
            InterstitialAd.Load(adData.Id, new AdRequest(), (ad, error) =>
            {
                if (ad is null) // 로드 실패
                {
                    TBMUtility.Log($"# Ads Manager - Interstitial Ad Load Failed");
                    TBMUtility.Log($"## error - {error.GetMessage()}");
                    TBMUtility.Log($"## responseInfo - {error.GetResponseInfo()}");
                    adData.InterstitialAd = null;
                    adData.AdState = AdState.LoadFailed;
                }
                else // 로드 성공
                {
                    TBMUtility.Log($"# Ads Manager - Interstitial Ad Load Succeed");
                    adData.InterstitialAd = ad;
                    adData.AdState = AdState.LoadSucceed;
                }
            });
        }

        private async UniTaskVoid CheckInterstitialAdLoadFinishedAsync(InterstitialAdData adData,
            Action<bool> resultCallback)
        {
            while (adData.AdState is AdState.Loading)
            {
                await UniTask.Yield(destroyCancellationToken);
            }

            resultCallback?.Invoke(adData.AdState is AdState.LoadSucceed);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="succeedCallback">true - 로드 성공, false - 로드 실패</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private void OnRewardedAdLoadRequested(string id, Action<bool> succeedCallback)
        {
            TBMUtility.Log($"# Ads Manager - Rewarded Ad({id}) Load Requested");
            if (_rewardedAdDataDict.TryGetValue(id, out var data) is false)
            {
                data = new RewardedAdData(id);
                _rewardedAdDataDict.Add(id, data);
            }

            switch (data.AdState)
            {
                case AdState.NotLoaded:
                case AdState.LoadFailed:
                    LoadRewardedAd(data, succeedCallback);
                    break;
                case AdState.Loading:
                    TBMUtility.Log($"## Ad is Loading now");
                    return;
                case AdState.LoadSucceed:
                case AdState.CanShow:
                    TBMUtility.Log($"## Ad has Already Loaded");
                    succeedCallback?.Invoke(true);
                    return;
                case AdState.Showing:
                    TBMUtility.Log($"## Ad is Showing now");
                    return;
                case AdState.ShowFailed:
                case AdState.ShowSucceed:
                    TBMUtility.Log($"## Ad Already Showed");
                    LoadRewardedAd(data, succeedCallback);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void LoadRewardedAd(RewardedAdData adData, Action<bool> succeedCallback)
        {
            adData.AdState = AdState.Loading;
            CheckRewardedAdLoadFinishedAsync(adData, succeedCallback).Forget();
            RewardedAd.Load(adData.Id, new AdRequest(),
                (ad, error) =>
                {
                    if (ad is null) // 로드 실패
                    {
                        TBMUtility.Log($"# Ads Manager - RewardedAd Ad Load Failed");
                        TBMUtility.Log($"## error - {error.GetMessage()}");
                        TBMUtility.Log($"## responseInfo - {error.GetResponseInfo()}");
                        adData.RewardedAd = null;
                        adData.AdState = AdState.LoadFailed;
                    }
                    else // 로드 성공
                    {
                        TBMUtility.Log($"# Ads Manager - RewardedAd Ad Load Succeed");
                        adData.RewardedAd = ad;
                        adData.AdState = AdState.LoadSucceed;
                    }
                }
            );
        }

        private async UniTaskVoid CheckRewardedAdLoadFinishedAsync(RewardedAdData adData, Action<bool> succeedCallback)
        {
            while (adData.AdState is AdState.Loading)
            {
                await UniTask.Yield(destroyCancellationToken);
            }

            succeedCallback?.Invoke(adData.AdState is AdState.LoadSucceed);
        }

        #endregion

        #region Can Show Methods

        private bool CanShowInterstitialAd(string id)
        {
            if (_interstitialAdDataDict.TryGetValue(id, out var interstitialAdData) is false)
            {
                return false;
            }

            if (interstitialAdData.InterstitialAd == null || interstitialAdData.InterstitialAd.CanShowAd() is false)
            {
                return false;
            }

            return true;
        }

        private bool CanShowRewardedAd(string id)
        {
            if (_rewardedAdDataDict.TryGetValue(id, out var rewardedAdData) is false)
            {
                return false;
            }

            if (rewardedAdData.RewardedAd == null || rewardedAdData.RewardedAd.CanShowAd() is false)
            {
                return false;
            }

            return true;
        }

        #endregion

        #region Show Ad Methods

        private void ShowInterstitialAd(string id)
        {
            TBMUtility.Log($"# Ads Manager : Show Interstitial Ad Requested");
            TBMUtility.Log($"## id - {id}");

            // 광고 데이터 있는지 체크
            if (_interstitialAdDataDict.TryGetValue(id, out var data) is false)
            {
                TBMUtility.Log($"## Interstitial Ad Data Not Found");
                return;
            }

            // 광고 보여줄 수 있는 상태인지 체크
            var interstitialAd = data.InterstitialAd;
            if ((data.AdState is AdState.LoadSucceed && interstitialAd.CanShowAd()) is false)
            {
                TBMUtility.Log($"## Interstitial Ad Can't Show.");
                TBMUtility.Log($"## AdState - {data.AdState}");
                return;
            }

            // 광고 출력
            data.AdState = AdState.Showing;
            interstitialAd.OnAdFullScreenContentOpened += OnAdOpened;
            interstitialAd.OnAdFullScreenContentClosed += () =>
            {
                OnAdClosed();
                data.AdState = AdState.ShowFailed;
                interstitialAd.Destroy();
            };
            interstitialAd.OnAdFullScreenContentFailed += error =>
            {
                OnAdFailed(error);
                data.AdState = AdState.ShowSucceed;
                interstitialAd.Destroy();
            };
            interstitialAd.Show();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="succeedCallback">true 시 보상 지급, false 시 문제 발생</param>
        private void ShowRewardedAd(string id, Action<bool> succeedCallback)
        {
            TBMUtility.Log($"# Ads Manager : Show Rewarded Ad Requested");
            TBMUtility.Log($"## id - {id}");

            // 광고 데이터 있는지 체크
            if (_rewardedAdDataDict.TryGetValue(id, out var rewardedAdData) is false)
            {
                TBMUtility.Log($"## Rewarded Ad Data Not Found");
                succeedCallback?.Invoke(false);
                return;
            }

            // 광고 보여줄 수 있는 상태인지 체크
            var rewardedAd = rewardedAdData.RewardedAd;
            if ((rewardedAdData.AdState is AdState.LoadSucceed && rewardedAd.CanShowAd()) is false)
            {
                TBMUtility.Log($"## Rewarded Ad Can't Show.");
                TBMUtility.Log($"## AdState - {rewardedAdData.AdState}");
                succeedCallback?.Invoke(false);
                return;
            }

            // 광고 출력
            rewardedAdData.AdState = AdState.Showing;
            rewardedAd.OnAdFullScreenContentOpened += OnAdOpened;
            rewardedAd.OnAdFullScreenContentClosed += () =>
            {
                OnAdClosed();
                if (rewardedAdData.AdState is not AdState.ShowSucceed)
                {
                    rewardedAdData.AdState = AdState.ShowFailed;
                }

                rewardedAd.Destroy();
            };
            rewardedAd.OnAdFullScreenContentFailed += error =>
            {
                OnAdFailed(error);
                rewardedAdData.AdState = AdState.ShowFailed;
                rewardedAd.Destroy();
            };

            CheckRewardedAdShowFinished(rewardedAdData, succeedCallback).Forget();
            rewardedAd.Show(_ => { rewardedAdData.AdState = AdState.ShowSucceed; });
        }

        private async UniTaskVoid CheckRewardedAdShowFinished(RewardedAdData adData, Action<bool> succeedCallback)
        {
            while (adData.AdState is AdState.Showing)
            {
                await UniTask.Yield(destroyCancellationToken);
            }

            succeedCallback?.Invoke(adData.AdState is AdState.ShowSucceed);
        }

        #endregion

        private void OnAdOpened()
        {
            MobileAds.SetApplicationMuted(true);
        }

        private void OnAdClosed()
        {
            MobileAds.SetApplicationMuted(false);
        }

        private void OnAdFailed(AdError error)
        {
            TBMUtility.Log($"# Ads Manager - Ad Failed");
            TBMUtility.Log($"## error - {error.GetMessage()}");
        }
    }
}