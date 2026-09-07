using System;
using MaouSuika.Core;
using UnityEngine;

namespace MaouSuika.Gameplay
{
    [CreateAssetMenu(fileName = "SoulOrb_PopEffect_DustConfig",
        menuName = MenuPath.Gameplay.POP_EFFECT + "DustConfig")]
    public class SoulOrbPopEffectDustConfigSO : SoulOrbPopEffectGradeConfigSO<SoulOrbPopEffectDustConfigSO.ColorProfile>
    {
        [Serializable]
        public struct ColorProfile
        {
            public Material dustMaterial;
            public ParticleSystem.MinMaxGradient dustLifetimeColor;
            public ParticleSystem.MinMaxGradient glowStartColor;
        }

        [SerializeField] private float dustEmissionCount = 40;
        [SerializeField] private float dustShapeRadius = 0.75f;
        [SerializeField] private float glowStartSize = 5f;

        public ParticleSystem.Burst GetDustBurst(float size) => new(0f, (short)(size * dustEmissionCount));
        public float GetDustRadius(float size) => size * dustShapeRadius;
        public float GetGlowStartSize(float size) => size * glowStartSize;
    }
}