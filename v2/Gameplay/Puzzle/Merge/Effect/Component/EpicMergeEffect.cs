using System;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    public class EpicMergeEffect : MergeEffectBase
    {
        [Serializable]
        public struct ColorProfile
        {
            public MergeEffectColor colorType;
            
            public ParticleSystem.MinMaxGradient explosionLifetimeGradient;
            public Material explosionMaterial;
            public Material ringMaterial;
            public ParticleSystem.MinMaxGradient glowStartGradient;
        }

        [SerializeField] private EpicMergeEffectConfigSO config;

        [SerializeField] private ParticleSystem explosionParticle;
        [SerializeField] private ParticleSystemRenderer explosionRenderer;
        [SerializeField] private ParticleSystem ringParticle;
        [SerializeField] private ParticleSystemRenderer ringRenderer;
        [SerializeField] private ParticleSystem glowParticle;

        protected override void SetSize(float size)
        {
            var explosionShape = explosionParticle.shape;
            explosionShape.radius = config.GetExplosionRadius(size);
            var explosionEmission = explosionParticle.emission;
            explosionEmission.SetBurst(0, config.GetExplosionBurst(size));

            var ringMain = ringParticle.main;
            ringMain.startSize = config.GetRingStartSize(size);
            var glowMain = glowParticle.main;
            glowMain.startSize = config.GetGlowStartSize(size);
        }

        protected override void SetColor(MergeEffectColor color)
        {
            var colorProfile = config.GetProfile(color);

            var explosionColorOverLifetime = explosionParticle.colorOverLifetime;
            explosionColorOverLifetime.color = colorProfile.explosionLifetimeGradient;
            explosionRenderer.sharedMaterial = colorProfile.explosionMaterial;

            ringRenderer.sharedMaterial = colorProfile.ringMaterial;

            var glowMain = glowParticle.main;
            glowMain.startColor = colorProfile.glowStartGradient;
        }
    }
}