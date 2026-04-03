using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using SOSG.Ads;
using SOSG.System;
using SOSG.System.Scene;
using SOSG.UGS.IAP;
using TaeBoMi;
using UnityEngine;

namespace SOSG.Stage
{
    public class StageInterstitialAds : MonoBehaviour
    {
        [SerializeField] private StageAdsSO stageAdsSO;
        [SerializeField] private IAPManagerSO iapManagerSO;
        [SerializeField] private AdsManagerSO adsManagerSO;

        private bool _haveRemoveAds;

        private CancellationTokenSource _reloadAdCts;

        private void Awake()
        {
            _reloadAdCts = new CancellationTokenSource();
            stageAdsSO.OnInterstitialAdRequested = ShowAds;
             SceneSetUpHelper.AddTask(Initialize);
        }

        private void OnDestroy()
        {
            stageAdsSO.OnInterstitialAdRequested = null;
            _reloadAdCts.CancelAndDispose();
        }

        private void Initialize()
        {
#if SOSG_DEBUG
            _haveRemoveAds = false;
#else
            _haveRemoveAds = iapManagerSO.IsPurchased(IAPProductId.RemoveInterstitialAds);
#endif
            if (_haveRemoveAds is false)
            {
                if (adsManagerSO.CanShowInterstitialAd(AdUnitId.GameOverInterstitialAdUnitId))
                {
                    adsManagerSO.ShowInterstitialAd(AdUnitId.GameOverInterstitialAdUnitId);
                }

                adsManagerSO.LoadInterstitialAd(AdUnitId.GameOverInterstitialAdUnitId, OnAdLoadFinished);
            }
            else
            {
                TBMUtility.Log("# GameOver Ads Manager - Ads Remover 보유중.");
            }
        }

        public void ShowAds()
        {
            if (_haveRemoveAds)
            {
                return;
            }

            adsManagerSO.ShowInterstitialAd(AdUnitId.GameOverInterstitialAdUnitId);
        }

        private void OnAdLoadFinished(bool success)
        {
            if (gameObject && success is false)
            {
                ReloadAd(_reloadAdCts.Token).Forget();
            }
        }

        private async UniTaskVoid ReloadAd(CancellationToken ct)
        {
            const double reloadDelay = 150;
            await UniTask.Delay(TimeSpan.FromSeconds(reloadDelay), cancellationToken: ct);
            adsManagerSO.LoadInterstitialAd(AdUnitId.GameOverInterstitialAdUnitId, OnAdLoadFinished);
        }
    }
}