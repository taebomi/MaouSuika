using Sirenix.OdinInspector;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    [CreateAssetMenu(menuName = "Maou Suika/Puzzle/Merge/Rare Effect Config")]
    public class RareMergeEffectConfigSO : MergeEffectBaseConfigSO<RareMergeEffect.ColorProfile>
    {
        [SerializeField]
        [MinMaxSlider(0f, 20f)] private Vector2 explosionStartSpeed = new(5f, 7.5f);

        [SerializeField] private int explosionBaseBurstCount = 13;

        [SerializeField] private float starRadius = 0.5f;
        [SerializeField] private int starBaseBurstCount = 10;

        [SerializeField] private float flicksRadius = 0.75f;
        [SerializeField] private int flicksBaseBurstCount = 15;

        [SerializeField] private float glowStartSize = 3f;

        public ParticleSystem.MinMaxCurve GetExplosionStartSpeed(float size) =>
            new(explosionStartSpeed.x * size, explosionStartSpeed.y * size);

        public ParticleSystem.Burst GetExplosionBurst(float size) => new(0f, (short)(size * explosionBaseBurstCount));
        public float GetStarRadius(float size) => starRadius * size;
        public ParticleSystem.Burst GetStarBurst(float size) => new(0f, (short)(size * starBaseBurstCount));
        public float GetFlicksRadius(float size) => flicksRadius * size;
        public ParticleSystem.Burst GetFlicksBurst(float size) => new(0f, (short)(size * flicksBaseBurstCount));
        public float GetGlowStartSize(float size) => glowStartSize * size;

        protected override MergeEffectColor GetColorType(RareMergeEffect.ColorProfile profile) => profile.colorType;
    }
}