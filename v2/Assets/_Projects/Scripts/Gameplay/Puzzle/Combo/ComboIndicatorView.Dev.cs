#if UNITY_EDITOR

using Sirenix.OdinInspector;
using UnityEngine;

namespace MaouSuika.Gameplay
{
    public partial class ComboIndicatorView
    {
        [Button]
        private void DEV_Play([PropertyRange(0, ComboGrades.MAX_MIN_COMBO)] int combo)
        {
            OnComboChanged(combo);
        }
    }
}

#endif