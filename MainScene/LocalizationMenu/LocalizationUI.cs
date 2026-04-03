using AYellowpaper.SerializedCollections;
using SOSG.System.Localization;
using SOSG.System.Setting;
using SOSG.System.UI;
using UnityEngine;
using UnityEngine.UI;

namespace SOSG.MainScene
{
    public class LocalizationUI : BaseUI, IModalUI
    {
        [SerializeField] private SerializedDictionary<string, Button> languageButtons;

        [SerializeField] private TitleLocalizer titleLocalizer;

        protected override void AwakeAfter()
        {
            Canvas.enabled = false;
        }

        public void Show()
        {
            UIHelper.ShowUI(this);
            Canvas.enabled = true;
            var curLocale = SettingDataHelper.InterfaceSetting.locale;
            languageButtons[curLocale].Select();
        }

        private void Hide()
        {
            Canvas.enabled = false;
            UIHelper.HideUI(this);
        }

        public void OnCloseRequested()
        {
            Hide();
        }

        public void OnOverlayClicked()
        {
            Hide();
        }

        public void OnLanguageChangeButtonClicked(string languageCode)
        {
            LocalizationHelper.ChangeLocale(languageCode, Hide);
            titleLocalizer.ChangeLocale(languageCode);
        }
    }
}