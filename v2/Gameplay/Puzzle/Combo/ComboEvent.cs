using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    public readonly struct ComboEvent
    {
        public readonly int Combo;
        public readonly Vector3 Position;

        public ComboEvent(int combo, Vector3 position)
        {
            Combo = combo;
            Position = position;
        }
    }

    public readonly struct ComboFailedEvent
    {
        public readonly int LastCombo;

        public ComboFailedEvent(int lastCombo)
        {
            LastCombo = lastCombo;
        }
    }
}