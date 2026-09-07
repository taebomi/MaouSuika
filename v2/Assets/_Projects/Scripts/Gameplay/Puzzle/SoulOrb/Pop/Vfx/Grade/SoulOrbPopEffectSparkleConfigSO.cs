using System;
using MaouSuika.Core;
using UnityEngine;

namespace MaouSuika.Gameplay
{
    [CreateAssetMenu(fileName = "SoulOrb_PopEffect_SparkleConfig",
        menuName = MenuPath.Gameplay.POP_EFFECT + "SparkleConfig")]
    public class
        SoulOrbPopEffectSparkleConfigSO : SoulOrbPopEffectGradeConfigSO<SoulOrbPopEffectSparkleConfigSO.ColorProfile>
    {
        [Serializable]
        public struct ColorProfile
        {
            public Material sharedMaterial;

            public ParticleSystem.MinMaxGradient explosionLifeTimeColor;
            public ParticleSystem.MinMaxGradient starLifeTimeColor;
            public ParticleSystem.MinMaxGradient flicksLifeTimeColor;

            public ParticleSystem.MinMaxGradient glowStartColor;
        }

        [SerializeField] private Vector2 explosionStartSpeed = new(5f, 7.5f);

        [SerializeField] private int explosionBaseBurstCount = 13;

        [SerializeField] private float starRadius = 0.5f;
        [SerializeField] private int starBaseBurstCount = 10;

        [SerializeField] private float flicksRadius = 0.75f;
        [SerializeField] private int flicksBaseBurstCount = 15;

        [SerializeField] private float glowStartSize = 3f;


        public ParticleSystem.MinMaxCurve GetExplosionStartSpeed(float size) =>
            new(explosionStartSpeed.x * size, explosionStartSpeed.y * size);

        public ParticleSystem.Burst GetExplosionBurst(float size) => new(0f, (short)(size * explosionBaseBurstCount));
        public float GetStarRadius(float size) => starRadius * size;
        public ParticleSystem.Burst GetStarBurst(float size) => new(0f, (short)(size * starBaseBurstCount));
        public float GetFlicksRadius(float size) => flicksRadius * size;
        public ParticleSystem.Burst GetFlicksBurst(float size) => new(0f, (short)(size * flicksBaseBurstCount));
        public float GetGlowStartSize(float size) => glowStartSize * size;
    }
}