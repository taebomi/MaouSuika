using System;
using System.Collections.Generic;
using Settings;
using TBM.MaouSuika.Core.Save;

namespace TBM.MaouSuika.Core.Settings
{
    [Serializable, ES3Serializable]
    public class SettingsData
    {
        public AudioSettings audio = new();
        public InputSettings input = new();
        public LocalizationSettings localization = new();
    }
}