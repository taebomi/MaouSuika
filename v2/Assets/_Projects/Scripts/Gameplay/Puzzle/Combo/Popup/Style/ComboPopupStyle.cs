using UnityEngine;

namespace MaouSuika.Gameplay
{
    public readonly struct ComboPopupStyle
    {
        public readonly float Scale;
        public readonly Color Color;
        public readonly float HoldTime;

        public ComboPopupStyle(float scale, Color color, float holdTime)
        {
            Scale = scale;
            Color = color;
            HoldTime = holdTime;
        }
    }
}