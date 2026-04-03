using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    [CreateAssetMenu(menuName = "Maou Suika/Puzzle/Merge/Epic Effect Config")]
    public class EpicMergeEffectConfigSO : MergeEffectBaseConfigSO<EpicMergeEffect.ColorProfile>
    {
        [SerializeField] private float explosionBaseRadius = 0.6f;
        [SerializeField] private int explosionBaseBurstEmission = 20;
        [SerializeField] private float ringStartSize = 3f;
        [SerializeField] private float glowStartSize = 4f;

        public float GetExplosionRadius(float size) => explosionBaseRadius * size;

        public ParticleSystem.Burst GetExplosionBurst(float size) => new(0, (short)(explosionBaseBurstEmission * size));
        public float GetRingStartSize(float size) => ringStartSize * size;
        public float GetGlowStartSize(float size) => glowStartSize * size;

        protected override MergeEffectColor GetColorType(EpicMergeEffect.ColorProfile profile) => profile.colorType;
    }
}