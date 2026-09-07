using MaouSuika.Core;
using UnityEngine;

namespace MaouSuika.Gameplay
{
    [CreateAssetMenu(fileName = "SoulOrb_TierDefinitionSO", menuName = MenuPath.Gameplay.SOUL_ORB + "Tier Definition")]
    public class SoulOrbTierDefinitionSO : ScriptableObject
    {
        [SerializeField] private float[] scales;
        [field: SerializeField] public int DrawableTierCount { get; private set; }

        public float GetScale(int tier) => scales[tier];

        private void OnValidate()
        {
            if (scales.Length != SoulOrbTiers.COUNT)
            {
                Debug.LogError($"must have {SoulOrbTiers.COUNT} entries.");
            }

            DrawableTierCount = Mathf.Clamp(DrawableTierCount, 1, SoulOrbTiers.COUNT);
        }
    }
}