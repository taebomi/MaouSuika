using UnityEngine;

namespace MaouSuika.Gameplay
{
    public class SoulOrbPopEffectImpact : SoulOrbPopEffectGradeBase
    {
        [SerializeField] private SoulOrbPopEffectImpactConfigSO config;

        [SerializeField] private ParticleSystem explosionParticles;
        [SerializeField] private ParticleSystemRenderer explosionRenderer;
        [SerializeField] private ParticleSystem centerParticles;
        [SerializeField] private ParticleSystemRenderer centerRenderer;
        [SerializeField] private ParticleSystem dustParticles;
        [SerializeField] private ParticleSystemRenderer dustRenderer;

        [SerializeField] private ParticleSystem glowParticles;

        private readonly ParticleSystem.Burst[] _bursts = new ParticleSystem.Burst[1];

        protected override void SetSize(float size)
        {
            var explosionMain = explosionParticles.main;
            explosionMain.startSize = config.GetExplosionStartSize(size);
            var explosionEmission = explosionParticles.emission;
            _bursts[0] = config.GetExplosionBurst(size);
            explosionEmission.SetBursts(_bursts);

            var centerMain = centerParticles.main;
            centerMain.startSize = config.GetCenterStartSize(size);

            var dustEmission = dustParticles.emission;
            _bursts[0] = config.GetDustBurst(size);
            dustEmission.SetBursts(_bursts);

            var glowMain = glowParticles.main;
            glowMain.startSize = config.GetGlowStartSize(size);
        }

        protected override void SetColor(SoulOrbPopEffectColor color)
        {
            var profile = config.GetColorProfile(color);

            explosionRenderer.sharedMaterial = profile.explosionMaterial;

            centerRenderer.sharedMaterial = profile.centerMaterial;

            dustRenderer.sharedMaterial = profile.dustMaterial;
            var dustColorOverLifeTime = dustParticles.colorOverLifetime;
            dustColorOverLifeTime.color = profile.dustLifeTimeColor;

            var glowMain = glowParticles.main;
            glowMain.startColor = profile.glowStartColor;
        }
    }
}