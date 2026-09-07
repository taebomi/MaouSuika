using System;
using UnityEngine;

namespace MaouSuika.Gameplay
{
    [Serializable]
    public struct ColorProfileEntry<TProfile>
    {
        public SoulOrbPopEffectColor color;
        public TProfile profile;
    }

    public abstract class SoulOrbPopEffectGradeConfigSO<TProfile> : ScriptableObject
    {
        [SerializeField] private ColorProfileEntry<TProfile>[] colorProfiles;

        public TProfile GetColorProfile(SoulOrbPopEffectColor color)
        {
            foreach (var colorProfile in colorProfiles)
            {
                if (colorProfile.color == color) return colorProfile.profile;
            }

            throw new ArgumentOutOfRangeException(nameof(color));
        }
    }
}