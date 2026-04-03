#if UNITY_EDITOR
using TBM;

namespace TBM.MaouSuika.Gameplay
{
    public partial class RegionProgressionSO
    {
        public RegionProgressionEntry[] Dev_Entries => entries;

        private void OnValidate()
        {
            for (var i = 1; i < entries.Length; i++)
            {
                if (entries[i].scoreThreshold <= entries[i - 1].scoreThreshold)
                    Logger.Error("Not Ordered By Ascending");
            }
        }
    }
}
#endif
