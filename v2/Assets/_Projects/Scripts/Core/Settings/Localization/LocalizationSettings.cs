using System;

namespace MaouSuika.Core.Localization
{
    [Serializable]
    public class LocalizationSettings
    {
        public string localeCode;

        public static LocalizationSettings CreateDefault()
        {
            return new LocalizationSettings();
        }

        public void Normalize()
        {
            localeCode = string.IsNullOrWhiteSpace(localeCode) ? null : localeCode.Trim();
        }

        public LocalizationSettings Copy()
        {
            return new LocalizationSettings
            {
                localeCode = localeCode,
            };
        }
    }
}
