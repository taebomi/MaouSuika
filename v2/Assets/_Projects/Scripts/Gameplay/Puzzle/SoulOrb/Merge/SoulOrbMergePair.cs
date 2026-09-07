using UnityEngine;

namespace MaouSuika.Gameplay
{
    public readonly struct SoulOrbMergePair
    {
        public SoulOrb Absorbed { get; }
        public SoulOrb Anchored { get; }

        public int Tier => Anchored.Tier;
        public Vector2 MergePosition => Anchored.RbPosition;
        public int CreationOrder => Anchored.CreationOrder;
        public float Size => Anchored.Scale;
        public bool HasLanded => Anchored.HasLanded || Absorbed.HasLanded;

        private SoulOrbMergePair(SoulOrb absorbed, SoulOrb anchored)
        {
            Absorbed = absorbed;
            Anchored = anchored;
        }

        public static SoulOrbMergePair Resolve(SoulOrb orbA, SoulOrb orbB)
        {
            Debug.Assert(orbA.CreationOrder != orbB.CreationOrder, "같은 Creation Order 면 안 돼!!!");

            return orbA.CreationOrder > orbB.CreationOrder
                ? new SoulOrbMergePair(orbA, orbB)
                : new SoulOrbMergePair(orbB, orbA);
        }
    }
}