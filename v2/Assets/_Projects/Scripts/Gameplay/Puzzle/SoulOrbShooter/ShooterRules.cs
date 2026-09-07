using UnityEngine;

namespace MaouSuika.Gameplay
{
    public static class ShooterRules
    {
        public const float MIN_POWER_RATIO = 0.05f;
        public const float MAX_POWER_RATIO = 1f;

        public static float ClampPowerRatio(float powerRatio)
        {
            return Mathf.Clamp(powerRatio, MIN_POWER_RATIO, MAX_POWER_RATIO);
        }
    }
}