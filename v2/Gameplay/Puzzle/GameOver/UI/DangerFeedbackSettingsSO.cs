using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    [CreateAssetMenu(fileName = "DangerFeedbackSettingsSO", menuName = "TBM/GameOver/Danger Feedback Settings")]
    public class DangerFeedbackSettingsSO : ScriptableObject
    {
        private static readonly int VignetteIntensity = Shader.PropertyToID("_VignetteIntensity");

        [field: SerializeField] public float TransitionDuration { get; private set; } = 1f;
        [field: SerializeField] public float FadeSpeed { get; private set; } = 1f;

        [field: SerializeField] public float VignetteIntensity_Warning { get; private set; } = 3.5f;
        [field: SerializeField] public float VignetteIntensity_Critical { get; private set; } = 1.25f;

        public void ApplyTo(Material material, float t)
        {
            if (material == null) return;

            material.SetFloat(VignetteIntensity,
                Mathf.Lerp(VignetteIntensity_Warning, VignetteIntensity_Critical, t));
        }

        public float GetIntensity(DangerLevel level)
        {
            return level >= DangerLevel.Critical ? 1f : 0f;
        }

        public float GetAlpha(DangerLevel level)
        {
            return level is DangerLevel.Safe ? 0f : 1f;
        }
    }
}