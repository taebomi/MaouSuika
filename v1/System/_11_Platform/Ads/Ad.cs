using GoogleMobileAds.Api;

namespace SOSG.Ads
{
    public class RewardedAdData
    {
        public readonly string Id;
        public RewardedAd RewardedAd;
        public AdState AdState;

        public RewardedAdData(string id)
        {
            Id = id;
            RewardedAd = null;
            AdState = AdState.NotLoaded;
        }
    }

    public class InterstitialAdData
    {
        public readonly string Id;
        public InterstitialAd InterstitialAd;
        public AdState AdState;

        public InterstitialAdData(string id)
        {
            Id = id;
            InterstitialAd = null;
            AdState = AdState.NotLoaded;
        }
    }
}