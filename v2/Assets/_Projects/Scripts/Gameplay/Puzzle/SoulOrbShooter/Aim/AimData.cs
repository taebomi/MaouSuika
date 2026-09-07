using System;
using UnityEngine;

namespace MaouSuika.Gameplay
{
    public readonly struct AimData : IEquatable<AimData>
    {
        public readonly Vector2 Direction;
        public readonly float PowerRatio;

        public AimData(Vector2 direction, float powerRatio)
        {
            Debug.Assert(Mathf.Approximately(direction.sqrMagnitude, 1f),
                $"Direction({direction}) not normalized.");
            Direction = direction;
            PowerRatio = powerRatio;
        }

        public bool Equals(AimData other) =>
            Direction.Equals(other.Direction) && PowerRatio.Equals(other.PowerRatio);

        public override bool Equals(object obj) => obj is AimData other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(Direction, PowerRatio);
    }
}