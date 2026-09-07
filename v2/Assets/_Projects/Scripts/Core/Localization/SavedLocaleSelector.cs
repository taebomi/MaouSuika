using System;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace MaouSuika.Core
{
    [Serializable]
    [DisplayName("Saved Locale Selector")]
    public class SavedLocaleSelector : IStartupLocaleSelector
    {
        [NonSerialized] private string _localeCode;

        public void SetLocaleCode(string localeCode)
        {
            _localeCode = localeCode;
        }

        public Locale GetStartupLocale(ILocalesProvider availableLocales)
        {
            if (string.IsNullOrWhiteSpace(_localeCode)) return null;

            return availableLocales.GetLocale(_localeCode);
        }
    }
}