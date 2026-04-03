using System;
using UnityEngine;

[CreateAssetMenu(menuName = "TaeBoMi/Ads/Stage Ads SO", fileName = "StageAdsSO")]
public class StageAdsSO : ScriptableObject
{
    public Action OnInterstitialAdRequested;
    public Action OnRewarded;
    
    public void ShowInterstitialAd()
    {
        OnInterstitialAdRequested?.Invoke();
    }
    
    public void NotifyRewarded()
    {
        OnRewarded?.Invoke();
    }
}
