using UnityEngine;

namespace MaouSuika.Gameplay
{
    public class SoulOrbPopEffectSparkle : SoulOrbPopEffectGradeBase
    {
        [SerializeField] private SoulOrbPopEffectSparkleConfigSO config;

        [SerializeField] private ParticleSystem explosionParticles;
        [SerializeField] private ParticleSystemRenderer explosionRenderer;
        [SerializeField] private ParticleSystem starParticles;
        [SerializeField] private ParticleSystemRenderer starRenderer;
        [SerializeField] private ParticleSystem flicksParticles;
        [SerializeField] private ParticleSystemRenderer flicksRenderer;
        [SerializeField] private ParticleSystem glowParticles;

        private readonly ParticleSystem.Burst[] _bursts = new ParticleSystem.Burst[1];

        protected override void SetSize(float size)
        {
            var explosionMain = explosionParticles.main;
            explosionMain.startSpeed = config.GetExplosionStartSpeed(size);
            var explosionEmission = explosionParticles.emission;
            _bursts[0] = config.GetExplosionBurst(size);
            explosionEmission.SetBursts(_bursts);

            var starShape = starParticles.shape;
            var starEmission = starParticles.emission;
            starShape.radius = config.GetStarRadius(size);
            _bursts[0] = config.GetStarBurst(size);
            starEmission.SetBursts(_bursts);

            var flicksShape = flicksParticles.shape;
            var flicksEmission = flicksParticles.emission;
            flicksShape.radius = config.GetFlicksRadius(size);
            _bursts[0] = config.GetFlicksBurst(size);
            flicksEmission.SetBursts(_bursts);

            var glowMain = glowParticles.main;
            glowMain.startSize = config.GetGlowStartSize(size);
        }

        protected override void SetColor(SoulOrbPopEffectColor color)
        {
            var colorProfile = config.GetColorProfile(color);

            explosionRenderer.sharedMaterial = colorProfile.sharedMaterial;
            starRenderer.sharedMaterial = colorProfile.sharedMaterial;
            flicksRenderer.sharedMaterial = colorProfile.sharedMaterial;

            var explosionColorOverLifeTime = explosionParticles.colorOverLifetime;
            explosionColorOverLifeTime.color = colorProfile.explosionLifeTimeColor;
            var starColorOverLifeTime = starParticles.colorOverLifetime;
            starColorOverLifeTime.color = colorProfile.starLifeTimeColor;
            var flicksColorOverLifeTime = flicksParticles.colorOverLifetime;
            flicksColorOverLifeTime.color = colorProfile.flicksLifeTimeColor;

            var glowMain = glowParticles.main;
            glowMain.startColor = colorProfile.glowStartColor;
        }
    }
}