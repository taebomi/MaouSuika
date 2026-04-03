namespace SOSG.UGS.IAP
{
    public static class IAPProductId
    {
        public const string Prefix = "com.taebomico.swipeoverlordsuikagame";
        public const string RemoveInterstitialAds = "removeinterstitialads";
        public const string Test001 = "test001";
        // public const string Prefix = "";
        // public const string RemoveInterstitialAds = "test";

        public static string GetFullId(string id)
        {
            return $"{Prefix}.{id}";
        }
    }
}