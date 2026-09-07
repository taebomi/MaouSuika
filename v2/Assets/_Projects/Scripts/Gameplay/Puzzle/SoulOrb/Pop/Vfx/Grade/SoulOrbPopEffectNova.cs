using UnityEngine;

namespace MaouSuika.Gameplay
{
    public class SoulOrbPopEffectNova : SoulOrbPopEffectGradeBase
    {
        [SerializeField] private SoulOrbPopEffectNovaConfigSO config;


        [SerializeField] private ParticleSystem explosionParticles;
        [SerializeField] private ParticleSystemRenderer explosionRenderer;
        [SerializeField] private ParticleSystem ringParticles;
        [SerializeField] private ParticleSystemRenderer ringRenderer;
        [SerializeField] private ParticleSystem glowParticles;

        private readonly ParticleSystem.Burst[] _bursts = new ParticleSystem.Burst[1];

        protected override void SetSize(float size)
        {
            var explosionShape = explosionParticles.shape;
            explosionShape.radius = config.GetExplosionRadius(size);
            var explosionEmission = explosionParticles.emission;
            _bursts[0] = config.GetExplosionBurst(size);
            explosionEmission.SetBursts(_bursts);

            var ringMain = ringParticles.main;
            ringMain.startSize = config.GetRingStartSize(size);
            var glowMain = glowParticles.main;
            glowMain.startSize = config.GetGlowStartSize(size);
        }

        protected override void SetColor(SoulOrbPopEffectColor color)
        {
            var colorProfile = config.GetColorProfile(color);

            var explosionColorOverLifetime = explosionParticles.colorOverLifetime;
            explosionColorOverLifetime.color = colorProfile.explosionLifetimeGradient;
            explosionRenderer.sharedMaterial = colorProfile.explosionMaterial;

            ringRenderer.sharedMaterial = colorProfile.ringMaterial;

            var glowMain = glowParticles.main;
            glowMain.startColor = colorProfile.glowStartColor;
        }
    }
}