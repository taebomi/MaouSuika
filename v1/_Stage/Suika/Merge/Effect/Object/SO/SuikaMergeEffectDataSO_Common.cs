using System;
using UnityEngine;

namespace SOSG.Stage.Suika.Merge
{
    [CreateAssetMenu(menuName = "SOSG/Suika/Merge/Common Effect Data",
        fileName = "SuikaMergeEffectDataSO_Common")]
    public class SuikaMergeEffectDataSO_Common : ScriptableObject
    {
        [field:SerializeField] public ColorData[] ColorDataArr { get; private set; }

        [Serializable]
        public class ColorData
        {
            // base
            public Material baseMaterial;
            public ParticleSystem.MinMaxGradient baseColorOverLifeTime;

            // light
            public Color lightColor;

            // glow
            public ParticleSystem.MinMaxGradient glowStartColor;
        }
    }
}