using System.Collections.Generic;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    public class SuikaTierDataTable
    {
        private readonly IReadOnlyList<SuikaTierData> _dataList;

        public int Count => _dataList.Count;
        public int MinTier => 0;
        public int MaxTier => _dataList.Count - 1;

        public SuikaTierDataTable(IReadOnlyList<SuikaTierData> dataList)
        {
            if (dataList == null || dataList.Count == 0)
            {
                throw new System.ArgumentException();
            }

            _dataList = dataList;
        }

        public SuikaTierData this[int tier]
        {
            get
            {
                if (!IsValidTier(tier))
                {
                    Logger.Warning($"Invalid Tier Request : {tier}");
                    tier = Mathf.Clamp(tier, MinTier, MaxTier);
                }

                return _dataList[tier];
            }
        }

        public bool IsValidTier(int tier) => tier >= MinTier && tier <= MaxTier;

        public bool IsMaxTier(int tier) => tier == MaxTier;

        public bool TryGetTierData(int tier, out SuikaTierData tierData)
        {
            if (IsValidTier(tier))
            {
                tierData = _dataList[tier];
                return true;
            }

            tierData = _dataList[0];
            return false;
        }

        public bool TryGetNextTierData(int currentTier, out SuikaTierData nextTierData)
        {
            return TryGetTierData(currentTier + 1, out nextTierData);
        }
    }
}