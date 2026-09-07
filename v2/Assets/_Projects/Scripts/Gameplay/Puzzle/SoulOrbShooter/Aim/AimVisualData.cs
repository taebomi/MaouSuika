using UnityEngine;

namespace MaouSuika.Gameplay
{
    public readonly struct AimVisualData
    {
        public readonly Vector2 Size;
        public readonly Color Color;

        public AimVisualData(Vector2 size, Color color)
        {
            Size = size;
            Color = color;
        }
    }
}