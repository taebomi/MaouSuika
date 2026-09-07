#if UNITY_EDITOR
using Sirenix.OdinInspector;
using UnityEngine;

namespace MaouSuika.Gameplay
{
    public partial class ComboPopupView
    {
        [Button]
        public void DEV_TestPlay(
            Vector2 position,
            [PropertyRange(0, ComboGrades.MAX_MIN_COMBO)] int combo)
        {
            Play(position, combo);
        }
    }
}
#endif
