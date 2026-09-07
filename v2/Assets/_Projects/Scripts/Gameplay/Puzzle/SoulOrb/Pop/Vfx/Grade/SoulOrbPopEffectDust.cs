using UnityEngine;

namespace MaouSuika.Gameplay
{
    public class SoulOrbPopEffectDust : SoulOrbPopEffectGradeBase
    {
        [SerializeField] private SoulOrbPopEffectDustConfigSO config;

        [SerializeField] private ParticleSystem dustParticles;
        [SerializeField] private ParticleSystemRenderer dustRenderer;
        [SerializeField] private ParticleSystem glowParticles;

        private readonly ParticleSystem.Burst[] _bursts = new ParticleSystem.Burst[1];

        protected override void SetSize(float size)
        {
            var dustEmission = dustParticles.emission;
            _bursts[0] = config.GetDustBurst(size);
            dustEmission.SetBursts(_bursts);
            var dustShape = dustParticles.shape;
            dustShape.radius = config.GetDustRadius(size);
            var glowMain = glowParticles.main;
            glowMain.startSize = config.GetGlowStartSize(size);
        }

        protected override void SetColor(SoulOrbPopEffectColor color)
        {
            var profile = config.GetColorProfile(color);

            dustRenderer.sharedMaterial = profile.dustMaterial;

            var dustColorOverLifetime = dustParticles.colorOverLifetime;
            dustColorOverLifetime.color = profile.dustLifetimeColor;

            var glowMain = glowParticles.main;
            glowMain.startColor = profile.glowStartColor;
        }
    }
}