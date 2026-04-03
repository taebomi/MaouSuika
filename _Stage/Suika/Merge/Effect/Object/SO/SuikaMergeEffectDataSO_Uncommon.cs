using System;
using UnityEngine;

namespace SOSG.Stage.Suika.Merge
{
    [CreateAssetMenu(menuName = "SOSG/Suika/Merge/Uncommon Effect Data",
        fileName = "SuikaMergeEffectDataSO_Uncommon")]
    public class SuikaMergeEffectDataSO_Uncommon : ScriptableObject
    {
        [field: SerializeField] public ColorData[] ColorDataArr { get; private set; }

        [Serializable]
        public class ColorData
        {
            // base
            public Material baseMaterial;

            // center
            public Material centerMaterial;

            // glow
            public ParticleSystem.MinMaxGradient glowStartColor;

            // dust
            public Material dustMaterial;
            public ParticleSystem.MinMaxGradient dustColorOverLifeTime;
        }
    }
}