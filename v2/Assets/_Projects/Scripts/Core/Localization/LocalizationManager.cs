using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using ProjectLocalizationSettings = MaouSuika.Core.Localization.LocalizationSettings;

namespace MaouSuika.Core
{
    public class LocalizationManager : MonoBehaviour
    {
        public static LocalizationManager Instance { get; private set; }

        public Locale CurrentLocale
        {
            get => LocalizationSettings.SelectedLocale;
            set => LocalizationSettings.SelectedLocale = value;
        }

        public string CurrentLocaleCode => CurrentLocale.Identifier.Code;
        public IReadOnlyList<Locale> AvailableLocales => LocalizationSettings.AvailableLocales.Locales;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            Instance = null;
        }

        public async UniTask InitializeAsync(string localeCode, CancellationToken token)
        {
            var selector = LocalizationSettings.StartupLocaleSelectors.OfType<SavedLocaleSelector>().Single();
            selector.SetLocaleCode(localeCode);
            await LocalizationSettings.InitializationOperation.ToUniTask(cancellationToken: token);
            Instance = this;
        }

        public void ApplySettings(ProjectLocalizationSettings settings)
        {
            if (string.IsNullOrWhiteSpace(settings.localeCode)) return;

            var locale = LocalizationSettings.AvailableLocales.GetLocale(settings.localeCode);
            if (locale != null) CurrentLocale = locale;
        }

        public ProjectLocalizationSettings CaptureSettings()
        {
            return new ProjectLocalizationSettings
            {
                localeCode = CurrentLocaleCode,
            };
        }

        private void OnDestroy()
        {
            if (ReferenceEquals(Instance, this) is false) return;

            Instance = null;
        }
    }
}
