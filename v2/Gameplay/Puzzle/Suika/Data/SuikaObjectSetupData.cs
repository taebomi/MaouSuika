namespace TBM.MaouSuika.Gameplay.Puzzle
{
    /// <summary>
    /// Suika Object Setup 용도
    /// </summary>
    public struct SuikaObjectSetupData
    {
        public readonly SuikaTierData TierData;
        public readonly int CreationOrder;

        public SuikaObjectSetupData(SuikaTierData tierData, int creationOrder)
        {
            TierData = tierData;
            CreationOrder = creationOrder;
        }
    }
}