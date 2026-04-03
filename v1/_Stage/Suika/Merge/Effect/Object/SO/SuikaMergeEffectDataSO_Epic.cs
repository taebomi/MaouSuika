using System;
using UnityEngine;

namespace SOSG.Stage.Suika.Merge
{
    [CreateAssetMenu(menuName = "SOSG/Suika/Merge/Epic Effect Data",
        fileName = "SuikaMergeEffectDataSO_Epic", order = 9000)]
    public class SuikaMergeEffectDataSO_Epic : ScriptableObject
    {
        [field: SerializeField] public ColorData[] ColorDataArr { get; private set; }

        [Serializable]
        public class ColorData
        {
            // base
            public Material baseMaterial;

            public ParticleSystem.MinMaxGradient baseColorOverLifeTime;

            // ring
            public Material ringMaterial;

            // glow
            public ParticleSystem.MinMaxGradient glowStartColor;
        }
    }
}