using System;
using MaouSuika.Core.Localization;
using MaouSuika.Gameplay;
using UnityEngine;

namespace MaouSuika.Core
{
    [Serializable]
    public class SettingsData
    {
        private const int CURRENT_SCHEMA_VERSION = 1;

        public int schemaVersion = CURRENT_SCHEMA_VERSION;
        public AudioSettings audioSettings = AudioSettings.CreateDefault();
        public InputProfile inputProfile = InputProfile.CreateDefault();
        public LocalizationSettings localizationSettings = LocalizationSettings.CreateDefault();

        public void Normalize()
        {
            audioSettings ??= AudioSettings.CreateDefault();
            audioSettings.Normalize();

            inputProfile ??= InputProfile.CreateDefault();
            inputProfile.Normalize();

            localizationSettings ??= LocalizationSettings.CreateDefault();
            localizationSettings.Normalize();
        }

        public SettingsData Copy()
        {
            return new SettingsData
            {
                schemaVersion = schemaVersion,
                audioSettings = audioSettings.Copy(),
                inputProfile = inputProfile.Copy(),
                localizationSettings = localizationSettings.Copy(),
            };
        }
    }
}
