using System;
using MaouSuika.Core;
using UnityEngine;

namespace MaouSuika.Gameplay
{
    [CreateAssetMenu(fileName = "SoulOrb_PopEffect_NovaConfig", menuName = MenuPath.Gameplay.POP_EFFECT + "NovaConfig")]
    public class
        SoulOrbPopEffectNovaConfigSO : SoulOrbPopEffectGradeConfigSO<SoulOrbPopEffectNovaConfigSO.ColorProfile>
    {
        [Serializable]
        public struct ColorProfile
        {
            public ParticleSystem.MinMaxGradient explosionLifetimeGradient;
            public Material explosionMaterial;
            public Material ringMaterial;
            public ParticleSystem.MinMaxGradient glowStartColor;
        }


        [SerializeField] private float explosionBaseRadius = 0.6f;
        [SerializeField] private int explosionBaseBurstEmission = 20;
        [SerializeField] private float ringStartSize = 3f;
        [SerializeField] private float glowStartSize = 4f;

        public float GetExplosionRadius(float size) => explosionBaseRadius * size;

        public ParticleSystem.Burst GetExplosionBurst(float size) => new(0, (short)(explosionBaseBurstEmission * size));
        public float GetRingStartSize(float size) => ringStartSize * size;
        public float GetGlowStartSize(float size) => glowStartSize * size;
    }
}