using System;
using UnityEngine;

namespace MaouSuika.Gameplay
{
    [Serializable]
    public partial class UnitDirectionalSpriteAnimation
    {
        [SerializeField] private Sprite[] left;
        [SerializeField] private Sprite[] right;

        [Min(1)]
        [SerializeField] private int fps = 10;
        [SerializeField] private bool loop;

        public int Fps => fps;
        public bool Loop => loop;
        public bool HasAnyFrame => Has(left) || Has(right);

        public UnitSpriteAnimation Resolve(UnitFacing facing)
        {
            var (primary, fallback) = facing is UnitFacing.Left ? (left, right) : (right, left);

            return Has(primary)
                ? new UnitSpriteAnimation(primary, fps, loop, false)
                : new UnitSpriteAnimation(fallback, fps, loop, true);
        }

        private static bool Has(Sprite[] frames) => frames is { Length: > 0 };
    }
}