using System;
using Cysharp.Threading.Tasks;
using SOSG.System.Setting;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace SOSG.System.Localization
{
    public class LocalizationManager : MonoBehaviour
    {
        public async UniTask SetUpAsync(InterfaceSetting interfaceSetting)
        {
            await ChangeLocaleAsync(interfaceSetting.locale);
        }


        public async UniTask ChangeLocaleAsync(string localeCode, Action onChanged = null)
        {
            TBMTimeScale.Pause(this);
            if (LocalizationSettings.InitializationOperation.IsDone is false)
            {
                await LocalizationSettings.InitializationOperation;
            }

            LocalizationSettings.SelectedLocale =
                LocalizationSettings.AvailableLocales.GetLocale(localeCode);
            SettingDataHelper.InterfaceSetting.SetLanguage(localeCode);

            TBMTimeScale.UnPause(this);
            onChanged?.Invoke();
        }
    }
}