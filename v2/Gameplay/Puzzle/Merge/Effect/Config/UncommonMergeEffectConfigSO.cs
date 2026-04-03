using Sirenix.OdinInspector;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    [CreateAssetMenu(menuName = "Maou Suika/Puzzle/Merge/Uncommon Effect Config")]
    public class UncommonMergeEffectConfigSO : MergeEffectBaseConfigSO<UncommonMergeEffect.ColorProfile>
    {
        [MinMaxSlider(0f, 5f)]
        [SerializeField] private Vector2 explosionBaseStartSize = new Vector2(1f, 1.25f);
        [SerializeField] private int explosionBaseBurstCount = 10;

        [SerializeField] private float centerBaseStartSize = 2f;

        [SerializeField] private int dustBaseBurstCount = 15;

        [SerializeField] private float glowBaseStartSize = 3f;

        public ParticleSystem.MinMaxCurve GetExplosionStartSize(float size) =>
            new(explosionBaseStartSize.x * size, explosionBaseStartSize.y * size);

        public ParticleSystem.Burst GetExplosionBurst(float size) => new(0f, (short)(size * explosionBaseBurstCount));

        public ParticleSystem.MinMaxCurve GetCenterStartSize(float size) => new(centerBaseStartSize * size);

        public ParticleSystem.Burst GetDustBurst(float size) => new(0f, (short)(size * dustBaseBurstCount));

        public ParticleSystem.MinMaxCurve GetGlowStartSize(float size) => new(glowBaseStartSize * size);

        protected override MergeEffectColor GetColorType(UncommonMergeEffect.ColorProfile profile) => profile.colorType;
    }
}