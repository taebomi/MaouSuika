using UnityEngine;

namespace MaouSuika.Gameplay
{
    public readonly struct SoulOrbMergeResult
    {
        public Vector2 Position { get; }
        public int SourceTier { get; }
        public int? ResultTier { get; }
        public float Size { get; }

        private SoulOrbMergeResult(SoulOrbMergePair pair, int? resultTier)
        {
            Position = pair.MergePosition;
            SourceTier = pair.Tier;
            ResultTier = resultTier;
            Size = pair.Size;
        }

        public static SoulOrbMergeResult Promote(SoulOrbMergePair pair)
        {
            return new SoulOrbMergeResult(pair, pair.Tier + 1);
        }

        public static SoulOrbMergeResult Finale(SoulOrbMergePair pair)
        {
            return new SoulOrbMergeResult(pair, null);
        }
    }
}