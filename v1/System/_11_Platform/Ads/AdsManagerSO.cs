using System;
using UnityEngine;

namespace SOSG.Ads
{
    [CreateAssetMenu(menuName = "TaeBoMi/Ads/Ads Manager", fileName = "AdsManagerSO")]
    public class AdsManagerSO : ScriptableObject
    {
        public Action<string, Action<bool>> OnInterstitialAdLoadRequested;
        public Action<string, Action<bool>> OnRewardedAdLoadRequested;
        public Func<string, bool> OnCanShowInterstitialAdRequested;
        public Func<string, bool> OnCanShowRewardedAdRequested;
        public Action<string> OnInterstitialAdShowRequested;
        public Action<string, Action<bool>> OnRewardedAdShowRequested;

        
        // Load
        public void LoadInterstitialAd(string id, Action<bool> loadCallback) =>
            OnInterstitialAdLoadRequested?.Invoke(id, loadCallback);
        public void LoadRewardedAd(string id, Action<bool> loadCallback) =>
            OnRewardedAdLoadRequested?.Invoke(id, loadCallback);

        // Can Show
        public bool CanShowInterstitialAd(string id) => OnCanShowInterstitialAdRequested?.Invoke(id) ?? false;
        public bool CanShowRewardedAd(string id) => OnCanShowRewardedAdRequested?.Invoke(id) ?? false;

        
        // Show
        public void ShowInterstitialAd(string id) => OnInterstitialAdShowRequested?.Invoke(id);

        public void ShowRewardedAd(string id, Action<bool> succeedCallback) =>
            OnRewardedAdShowRequested?.Invoke(id, succeedCallback);
    }
}