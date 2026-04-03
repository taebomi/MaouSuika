using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    public readonly struct AimData
    {
        public readonly Vector2 Direction;
        public readonly float PowerRatio;

        public AimData(Vector2 direction, float powerRatio)
        {
            Direction = direction;
            PowerRatio = powerRatio;
        }

        public static readonly AimData Default = new(Vector2.up, 0.5f);
    }
}