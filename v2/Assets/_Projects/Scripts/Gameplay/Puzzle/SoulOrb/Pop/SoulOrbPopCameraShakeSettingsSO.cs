using System;
using MaouSuika.Core;
using UnityEngine;

namespace MaouSuika.Gameplay
{
    [CreateAssetMenu(fileName = "SoulOrbPopCameraShakeSettingsSO",
        menuName = MenuPath.Gameplay.POP_EFFECT + "Camera Shake Settings")]
    public class SoulOrbPopCameraShakeSettingsSO : ScriptableObject
    {
        [SerializeField] private AnimationCurve strengthByTier;
        [SerializeField] private AnimationCurve durationByTier;
        [SerializeField] private AnimationCurve minimumTierByCombo;

        public CameraShakeRequest RequestFor(int tier, int combo)
        {
            combo = Mathf.Clamp(combo, 0, ComboGrades.MAX_MIN_COMBO);

            var minimumTier = minimumTierByCombo.Evaluate(combo);
            var effectiveTier = Mathf.Max(tier, minimumTier);

            var strength = strengthByTier.Evaluate(effectiveTier);
            var duration = durationByTier.Evaluate(effectiveTier);

            return new CameraShakeRequest(strength, duration);
        }
    }
}