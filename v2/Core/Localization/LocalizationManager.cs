using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace TBM.MaouSuika.Core.Localization
{
    public class LocalizationManager : CoreManager<LocalizationManager>
    {
        public async UniTask InitializeAsync()
        {
            await LocalizationSettings.InitializationOperation;
        }

        public void Deinitialize()
        {
        }

        #region Locale

        public Locale CurrentLocale
        {
            get => LocalizationSettings.SelectedLocale;
            private set => LocalizationSettings.SelectedLocale = value;
        }
        public string CurrentLocaleCode => CurrentLocale.Identifier.Code;
        public List<Locale> AvailableLocales => LocalizationSettings.AvailableLocales.Locales;

        public void SetLocale(Locale locale)
        {
            CurrentLocale = locale;
        }

        #endregion
    }
}