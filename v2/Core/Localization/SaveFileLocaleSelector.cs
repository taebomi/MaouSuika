using System;
using TBM.MaouSuika.Core.Save;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using LocalizationSettings = UnityEngine.Localization.Settings.LocalizationSettings;

namespace TBM.MaouSuika.Core.Localization
{
    [Serializable]
    public class SaveFileLocaleSelector : IStartupLocaleSelector
    {
        public Locale GetStartupLocale(ILocalesProvider availableLocales)
        {
            var storage = new SettingsStorage();
            var data = storage.Load();
            var languageCode = data.localization.languageCode;
            if (string.IsNullOrEmpty(languageCode)) return null;

            return LocalizationSettings.AvailableLocales.GetLocale(languageCode);
        }
    }
}