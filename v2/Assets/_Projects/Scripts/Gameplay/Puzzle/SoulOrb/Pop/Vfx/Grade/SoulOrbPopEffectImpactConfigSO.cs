using System;
using MaouSuika.Core;
using UnityEngine;

namespace MaouSuika.Gameplay
{
    [CreateAssetMenu(fileName = "SoulOrb_PopEffect_ImpactConfig",
        menuName = MenuPath.Gameplay.POP_EFFECT + "ImpactConfig")]
    public class
        SoulOrbPopEffectImpactConfigSO : SoulOrbPopEffectGradeConfigSO<SoulOrbPopEffectImpactConfigSO.ColorProfile>
    {
        [Serializable]
        public struct ColorProfile
        {
            public Material explosionMaterial;

            public Material centerMaterial;

            public Material dustMaterial;
            public ParticleSystem.MinMaxGradient dustLifeTimeColor;

            public ParticleSystem.MinMaxGradient glowStartColor;
        }

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
    }
}