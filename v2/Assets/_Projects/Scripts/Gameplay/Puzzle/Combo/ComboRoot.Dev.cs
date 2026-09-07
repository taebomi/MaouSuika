#if UNITY_EDITOR

using Sirenix.OdinInspector;
using UnityEngine;

namespace MaouSuika.Gameplay
{
    public partial class ComboRoot
    {
        [Button]
        [EnableIf("@UnityEngine.Application.isPlaying")]
        private void DEV_AddCombo()
        {
            _combo.Add();
        }

        [Button]
        [EnableIf("@UnityEngine.Application.isPlaying")]
        private void DEV_ResetCombo()
        {
            _combo.CheckPoint();
        }

        private void DEV_PlayPopupEffect(Vector2 position, int combo)
        {
            comboPopupView.DEV_TestPlay(position, combo);
        }
    }
}

#endif
