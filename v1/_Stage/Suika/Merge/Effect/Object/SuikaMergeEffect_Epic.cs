using SOSG.Monster;
using UnityEngine;
using UnityEngine.Serialization;

namespace SOSG.Stage.Suika.Merge
{
    public class SuikaMergeEffect_Epic : SuikaMergeEffectBase
    {
        public override MonsterGrade Grade => MonsterGrade.Epic;

        [SerializeField] private SuikaMergeEffectDataSO_Epic data;

        [SerializeField] private ParticleSystem basePS;
        [SerializeField] private ParticleSystemRenderer basePSRenderer;
        [SerializeField] private ParticleSystem ringPS;
        [SerializeField] private ParticleSystemRenderer ringPSRenderer;
        [SerializeField] private ParticleSystem glowPS;

        private const float BaseRadius = 0.6f;
        private const int BaseBurstCount = 20;
        private const float RingStartSize = 3f;
        private const float GlowStartSize = 4f;

        public override void SetSize(float size)
        {
            var baseShape = basePS.shape;
            baseShape.radius = BaseRadius * size;
            var baseEmission = basePS.emission;
            baseEmission.SetBurst(0, new ParticleSystem.Burst(0f, (short)(size * BaseBurstCount)));

            var ringMain = ringPS.main;
            ringMain.startSize = RingStartSize * size;

            var glowMain = glowPS.main;
            glowMain.startSize = GlowStartSize * size;
        }

        public override void SetColor(MonsterDataSO.MergeEffectColor color)
        {
            var curData = data.ColorDataArr[(int)color];

            // base
            var baseColorOverLifeTime = basePS.colorOverLifetime;
            baseColorOverLifeTime.color = curData.baseColorOverLifeTime;
            basePSRenderer.material = curData.baseMaterial;

            // ring
            ringPSRenderer.material = curData.ringMaterial;

            // glow
            var glowMain = glowPS.main;
            glowMain.startColor = curData.glowStartColor;
        }
    }
}