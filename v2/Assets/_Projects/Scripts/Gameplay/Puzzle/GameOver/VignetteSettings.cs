using System;
using UnityEngine;

namespace MaouSuika.Gameplay
{
    [Serializable]
    public struct VignetteSettings
    {
        public float baseRadius;
        public float softness;
        public float pulseSpeed;
        public float pulseAmount;
        public float intensity;
        public float flowSpeed;

        private static readonly int BaseRadiusID = Shader.PropertyToID("_BaseRadius");
        private static readonly int SoftnessID = Shader.PropertyToID("_Softness");
        private static readonly int PulseSpeedID = Shader.PropertyToID("_PulseSpeed");
        private static readonly int PulseAmountID = Shader.PropertyToID("_PulseAmount");
        private static readonly int IntensityID = Shader.PropertyToID("_Intensity");
        private static readonly int FlowSpeedID = Shader.PropertyToID("_FlowSpeed");

        public readonly void ApplyTo(Material mat)
        {
            mat.SetFloat(BaseRadiusID, baseRadius);
            mat.SetFloat(SoftnessID, softness);
            mat.SetFloat(PulseAmountID, pulseAmount);
            mat.SetFloat(IntensityID, intensity);
        }

        public static VignetteSettings Lerp(in VignetteSettings a, in VignetteSettings b, float t) => new()
        {
            baseRadius = Mathf.Lerp(a.baseRadius, b.baseRadius, t),
            softness = Mathf.Lerp(a.softness, b.softness, t),
            flowSpeed = Mathf.Lerp(a.flowSpeed, b.flowSpeed, t),
            pulseAmount = Mathf.Lerp(a.pulseAmount, b.pulseAmount, t),
            intensity = Mathf.Lerp(a.intensity, b.intensity, t),
            pulseSpeed = Mathf.Lerp(a.pulseSpeed, b.pulseSpeed, t),
        };
    }
}