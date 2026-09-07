using UnityEngine;

namespace MaouSuika.Gameplay
{
    public readonly struct ComboPopupRequest
    {
        public readonly Vector2 Position;
        public readonly string Text;

        public readonly ComboPopupMotionSO Motion;
        public readonly ComboPopupStyle Style;

        public ComboPopupRequest(Vector2 position, string text, ComboPopupMotionSO motion, ComboPopupStyle style)
        {
            Position = position;
            Text = text;
            Motion = motion;
            Style = style;
        }
    }
}