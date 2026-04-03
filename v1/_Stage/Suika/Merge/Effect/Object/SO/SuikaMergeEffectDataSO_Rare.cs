using System;
using UnityEngine;

namespace SOSG.Stage.Suika.Merge
{
    [CreateAssetMenu(menuName = "SOSG/Suika/Merge/Rare Effect Data",
        fileName = "SuikaMergeEffectDataSO_Rare")]
    public class SuikaMergeEffectDataSO_Rare : ScriptableObject
    {
        [field: SerializeField] public ColorData[] ColorDataArr { get; private set; }


        [Serializable]
        public class ColorData
        {
            // 공용
            public Material material;

            // base
            public ParticleSystem.MinMaxGradient baseColorOverLifeTime;

            // bigStar
            public ParticleSystem.MinMaxGradient starColorOverLifeTime;

            // Flicks
            public ParticleSystem.MinMaxGradient flicksColorOverLifeTime;

            // light
            public Color lightColor;

            // bigGlow
            public ParticleSystem.MinMaxGradient glowStartColor;
        }
    }
}