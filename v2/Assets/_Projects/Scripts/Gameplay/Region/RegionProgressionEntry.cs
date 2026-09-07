using System;
using UnityEngine;

namespace MaouSuika.Gameplay
{
    [Serializable]
    public struct RegionProgressionEntry
    {
        [SerializeField, Min(0)] private int requiredTotalScore;
        [SerializeField] private RegionDefinitionSO region;

        public int RequiredTotalScore => requiredTotalScore;
        public RegionDefinitionSO Region => region;
    }
}