using UnityEngine;

namespace MaouSuika.Gameplay
{
    public readonly struct SoulOrbQueueSlotData
    {
        public readonly int Tier;
        public readonly Sprite Icon;
        public readonly Color Color;

        public SoulOrbQueueSlotData(int tier, Sprite icon, Color color)
        {
            Tier = tier;
            Icon = icon;
            Color = color;
        }
    }
}