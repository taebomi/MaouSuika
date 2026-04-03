using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    [CreateAssetMenu(menuName = "Maou Suika/Puzzle/Merge/Common Effect Config", fileName = "CommonMergeEffectConfigSO")]
    public class CommonMergeEffectConfigSO : MergeEffectBaseConfigSO<CommonMergeEffect.ColorProfile>
    {
        [SerializeField] private float explosionEmissionCount = 40;
        [SerializeField] private float explosionShapeRadius = 0.75f;
        [SerializeField] private float glowStartSize = 5f;

        protected override MergeEffectColor GetColorType(CommonMergeEffect.ColorProfile profile) => profile.colorType;

        public ParticleSystem.Burst GetExplosionBurst(float size) => new(0f, (short)size * explosionEmissionCount);
        public float GetExplosionRadius(float size) => size * explosionShapeRadius;
        public float GetGlowStartSize(float size) => glowStartSize * size;
    }
}