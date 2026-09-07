using System;
using UnityEngine;

namespace MaouSuika.Gameplay
{
    [Serializable]
    public class ShooterDragInputSettings
    {
        // Unit 단위
        public const float MIN_DRAG_RANGE = 1f;
        public const float MAX_DRAG_RANGE = 6f;

        [Min(MIN_DRAG_RANGE)]
        public float dragRange;
        public bool isSlingshot;


        public ShooterDragInputSettings(float dragRange, bool isSlingshot)
        {
            this.dragRange = dragRange;
            this.isSlingshot = isSlingshot;
        }

        public void CopyFrom(ShooterDragInputSettings source)
        {
            dragRange = source.dragRange;
            isSlingshot = source.isSlingshot;
        }

        public void Normalize(ShooterDragInputSettings defaults)
        {
            if (float.IsNaN(dragRange) || float.IsInfinity(dragRange) || dragRange < MIN_DRAG_RANGE)
                dragRange = defaults.dragRange;

            dragRange = Mathf.Clamp(dragRange, MIN_DRAG_RANGE, MAX_DRAG_RANGE);
        }
    }
}