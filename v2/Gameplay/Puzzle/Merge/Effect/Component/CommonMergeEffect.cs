using System;
using TBM.MaouSuika.Gameplay.Unit;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    public class CommonMergeEffect : MergeEffectBase
    {
        [Serializable]
        public struct ColorProfile
        {
            public MergeEffectColor colorType;

            public Material explosionMaterial;
            public ParticleSystem.MinMaxGradient explosionLifetimeColor;
            public ParticleSystem.MinMaxGradient glowStartColor;
        }

        [SerializeField] private CommonMergeEffectConfigSO config;

        [SerializeField] private ParticleSystem explosionParticle;
        [SerializeField] private ParticleSystemRenderer explosionParticleRenderer;
        [SerializeField] private ParticleSystem glowParticle;


        protected override void SetSize(float size)
        {
            var explosionEmission = explosionParticle.emission;
            explosionEmission.SetBurst(0, config.GetExplosionBurst(size));
            var explosionShape = explosionParticle.shape;
            explosionShape.radius = config.GetExplosionRadius(size);
            var glowMain = glowParticle.main;
            glowMain.startSize = config.GetGlowStartSize(size);
        }

        protected override void SetColor(MergeEffectColor color)
        {
            var profile = config.GetProfile(color);

            explosionParticleRenderer.sharedMaterial = profile.explosionMaterial;

            var explosionColorOverLifetime = explosionParticle.colorOverLifetime;
            explosionColorOverLifetime.color = profile.explosionLifetimeColor;

            var glowMain = glowParticle.main;
            glowMain.startColor = profile.glowStartColor;
        }
    }
}