using System;
using TBM.MaouSuika.Gameplay.Unit;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    public class UncommonMergeEffect : MergeEffectBase
    {
        [Serializable]
        public struct ColorProfile
        {
            public MergeEffectColor colorType;

            public Material explosionMaterial;
            public Material centerMaterial;
            public Material dustMaterial;
            public ParticleSystem.MinMaxGradient dustLifeTimeColor;

            public ParticleSystem.MinMaxGradient glowStartColor;
        }

        [SerializeField] private UncommonMergeEffectConfigSO config;

        [SerializeField] private ParticleSystem explosionParticle;
        [SerializeField] private ParticleSystemRenderer explosionParticleRenderer;
        [SerializeField] private ParticleSystem centerParticle;
        [SerializeField] private ParticleSystemRenderer centerParticleRenderer;
        [SerializeField] private ParticleSystem dustParticle;
        [SerializeField] private ParticleSystemRenderer dustParticleRenderer;

        [SerializeField] private ParticleSystem glowParticle;

        protected override void SetSize(float size)
        {
            var explosionMain = explosionParticle.main;
            explosionMain.startSize = config.GetExplosionStartSize(size);
            var explosionEmission = explosionParticle.emission;
            explosionEmission.SetBurst(0, config.GetExplosionBurst(size));

            var centerMain = centerParticle.main;
            centerMain.startSize = config.GetCenterStartSize(size);

            var dustEmission = dustParticle.emission;
            dustEmission.SetBurst(0, config.GetDustBurst(size));

            var glowMain = glowParticle.main;
            glowMain.startSize = config.GetGlowStartSize(size);
        }

        protected override void SetColor(MergeEffectColor color)
        {
            var profile = config.GetProfile(color);

            explosionParticleRenderer.sharedMaterial = profile.explosionMaterial;


            centerParticleRenderer.sharedMaterial = profile.centerMaterial;

            dustParticleRenderer.sharedMaterial = profile.dustMaterial;
            var dustColorOverLifeTime = dustParticle.colorOverLifetime;
            dustColorOverLifeTime.color = profile.dustLifeTimeColor;

            var glowMain = glowParticle.main;
            glowMain.startColor = profile.glowStartColor;
        }
    }
}