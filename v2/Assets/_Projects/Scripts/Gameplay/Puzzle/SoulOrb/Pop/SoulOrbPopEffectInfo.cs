using UnityEngine;

namespace MaouSuika.Gameplay
{
    public readonly struct SoulOrbPopEffectInfo
    {
        public readonly Vector2 Position;
        public readonly float Size;
        public readonly int Tier;
        public readonly SoulOrbPopEffectColor Color;
        public readonly int Combo;

        public Grade Grade => SoulOrbTiers.GradeOf(Tier);

        public SoulOrbPopEffectInfo(
            Vector2 position, float size, int tier, SoulOrbPopEffectColor color, int combo)
        {
            Position = position;
            Size = size;
            Tier = tier;
            Color = color;
            Combo = combo;
        }

        public SoulOrbPopEffectInfo(SoulOrb orb, int combo) : this(
            orb.transform.position,
            orb.Scale,
            orb.Tier,
            orb.MonsterData.soulOrbPopEffectColor,
            combo) { }

        public SoulOrbPopEffectInfo(in SoulOrbSnapshot snapshot, int combo) : this(
            snapshot.Position,
            snapshot.Size,
            snapshot.Tier,
            snapshot.MonsterData.soulOrbPopEffectColor,
            combo) { }
    }
}