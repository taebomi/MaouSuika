using System;
using TBM.MaouSuika.Gameplay.Unit;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    public class RareMergeEffect : MergeEffectBase
    {
        [Serializable]
        public struct ColorProfile
        {
            public MergeEffectColor colorType;

            public Material sharedMaterial;

            public ParticleSystem.MinMaxGradient explosionLifeTimeColor;
            public ParticleSystem.MinMaxGradient starLifeTimeColor;
            public ParticleSystem.MinMaxGradient flicksLifeTimeColor;

            public ParticleSystem.MinMaxGradient glowStartColor;
        }

        [SerializeField] private RareMergeEffectConfigSO config;

        [SerializeField] private ParticleSystem explosionParticle;
        [SerializeField] private ParticleSystemRenderer explosionParticleRenderer;
        [SerializeField] private ParticleSystem starParticle;
        [SerializeField] private ParticleSystemRenderer starParticleRenderer;
        [SerializeField] private ParticleSystem flicksParticle;
        [SerializeField] private ParticleSystemRenderer flicksParticleRenderer;
        [SerializeField] private ParticleSystem glowParticle;

        protected override void SetSize(float size)
        {
            var explosionMain = explosionParticle.main;
            explosionMain.startSpeed = config.GetExplosionStartSpeed(size);
            var explosionEmission = explosionParticle.emission;
            explosionEmission.SetBurst(0, config.GetExplosionBurst(size));

            var starMain = starParticle.main;
            var starEmission = starParticle.emission;
            starMain.startSize = config.GetStarRadius(size);
            starEmission.SetBurst(0, config.GetStarBurst(size));

            var flicksMain = flicksParticle.main;
            var flicksEmission = flicksParticle.emission;
            flicksMain.startSize = config.GetFlicksRadius(size);
            flicksEmission.SetBurst(0, config.GetFlicksBurst(size));

            var glowMain = glowParticle.main;
            glowMain.startSize = config.GetGlowStartSize(size);
        }

        protected override void SetColor(MergeEffectColor color)
        {
            var colorProfile = config.GetProfile(color);

            explosionParticleRenderer.sharedMaterial = colorProfile.sharedMaterial;
            starParticleRenderer.sharedMaterial = colorProfile.sharedMaterial;
            flicksParticleRenderer.sharedMaterial = colorProfile.sharedMaterial;

            var explosionColorOverLifeTime = explosionParticle.colorOverLifetime;
            explosionColorOverLifeTime.color = colorProfile.explosionLifeTimeColor;
            var starColorOverLifeTime = starParticle.colorOverLifetime;
            starColorOverLifeTime.color = colorProfile.starLifeTimeColor;
            var flicksColorOverLifeTime = flicksParticle.colorOverLifetime;
            flicksColorOverLifeTime.color = colorProfile.flicksLifeTimeColor;

            var glowMain = glowParticle.main;
            glowMain.startColor = colorProfile.glowStartColor;
        }
    }
}