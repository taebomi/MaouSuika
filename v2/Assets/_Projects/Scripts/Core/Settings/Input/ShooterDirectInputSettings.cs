using System;
using UnityEngine;

namespace MaouSuika.Gameplay
{
    [Serializable]
    public class ShooterDirectInputSettings
    {
        public bool isSlingshot;

        public ShooterDirectInputSettings(bool isSlingshot)
        {
            this.isSlingshot = isSlingshot;
        }

        public void CopyFrom(ShooterDirectInputSettings settings)
        {
            isSlingshot = settings.isSlingshot;
        }
    }
}