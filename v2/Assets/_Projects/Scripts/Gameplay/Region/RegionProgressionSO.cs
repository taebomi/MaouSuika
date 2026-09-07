using System.Collections.Generic;
using MaouSuika.Core;
using UnityEngine;

namespace MaouSuika.Gameplay
{
    [CreateAssetMenu(fileName = "RegionProgression", menuName = MenuPath.Gameplay.Region + "Progression")]
    public class RegionProgressionSO : ScriptableObject
    {
        [SerializeField] private RegionProgressionEntry[] entries;

        public IReadOnlyList<RegionProgressionEntry> Entries => entries;
    }
}