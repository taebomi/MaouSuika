using UnityEngine;

namespace MaouSuika.Gameplay
{
    public readonly struct SoulOrbSnapshot
    {
        public readonly Vector2 Position;
        public readonly float Size;
        public readonly int Tier;
        public readonly MonsterDataSO MonsterData;

        public SoulOrbSnapshot(SoulOrb soulOrb)
        {
            Position = soulOrb.transform.position;
            Size = soulOrb.Scale;
            Tier = soulOrb.Tier;
            MonsterData = soulOrb.MonsterData;
        }
    }
}