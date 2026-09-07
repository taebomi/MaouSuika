using System;
using UnityEngine;

namespace MaouSuika.Gameplay
{
    [Serializable]
    public class ShooterControlSchemeSettings
    {
        public ShooterControlMode controlMode;
        public ShooterDirectInputSettings direct;
        public ShooterDragInputSettings drag;


        public void CopyFrom(ShooterControlSchemeSettings source)
        {
            controlMode = source.controlMode;

            direct.CopyFrom(source.direct);
            drag.CopyFrom(source.drag);
        }

        public void Normalize(ShooterControlSchemeSettings defaults)
        {
            if (!Enum.IsDefined(typeof(ShooterControlMode), controlMode)) controlMode = defaults.controlMode;

            direct ??= defaults.direct;
            drag ??= defaults.drag;

            drag.Normalize(defaults.drag);
        }
    }
}