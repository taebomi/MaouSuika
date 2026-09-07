using System;
using UnityEngine;

namespace MaouSuika.Gameplay
{
    public class SingleSoulOrbTargetingView : MonoBehaviour
    {
        private SoulOrb _target;

        public void Show(SoulOrb target)
        {
            if (_target == target)
                return;

            Hide();

            _target = target;

            if (_target != null)
                _target.SetOutline(true);
        }

        public void Hide()
        {
            if (_target != null)
                _target.SetOutline(false);

            _target = null;
        }
    }
}