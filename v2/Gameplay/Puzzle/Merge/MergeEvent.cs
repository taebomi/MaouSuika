using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    public readonly struct MergeEvent
    {
        public Vector3 Pos { get; init; }
        public float Size { get; init; }

        public int Tier { get; init; }
        public int CreationOrder { get; init; }
        public bool IsGrounded { get; init; }

        public MergeEffectGrade EffectGrade { get; init; }
        public MergeEffectColor EffectColor { get; init; }

        public bool HasResult { get; init; }
        public int ResultTier { get; init; }
    }
}