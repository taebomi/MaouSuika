using System.Collections.Generic;
using System.Linq;
using MaouSuika.Core;
using MaouSuika.Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Pseudo;
using UnityEngine.UI;

namespace MaouSuika.Scripts
{
    public class SettingsView : MonoBehaviour
    {
        [Header("Tabs")]
        [SerializeField] private Button keyboardMouseTabButton;
        [SerializeField] private Button gamepadTabButton;

        [Header("Panels")]
        [SerializeField] private GameObject keyboardMousePanel;
        [SerializeField] private GameObject gamepadPanel;

        [Header("Keyboard & Mouse")]
        [SerializeField] private Slider keyboardMouseDragRangeSlider;
        [SerializeField] private Toggle keyboardMouseSlingshotToggle;

        [Header("Gamepad")]
        [SerializeField] private Toggle gamepadSlingshotToggle;

        [Header("Audio")]
        [SerializeField] private Slider masterVolumeSlider;
        [SerializeField] private Toggle masterMutedToggle;
        [SerializeField] private Slider bgmVolumeSlider;
        [SerializeField] private Toggle bgmMutedToggle;
        [SerializeField] private Slider sfxVolumeSlider;
        [SerializeField] private Toggle sfxMutedToggle;

        [SerializeField] private TMP_Dropdown languageDropdown;

        private SettingsData _draft;

        private readonly List<Locale> _locales = new List<Locale>();

        private void Awake()
        {
            keyboardMouseTabButton.onClick.AddListener(ShowKeyboardMouseTab);
            gamepadTabButton.onClick.AddListener(ShowGamepadTab);

            keyboardMouseDragRangeSlider.minValue =
                ShooterDragInputSettings.MIN_DRAG_RANGE;

            keyboardMouseDragRangeSlider.maxValue =
                ShooterDragInputSettings.MAX_DRAG_RANGE;

            keyboardMouseDragRangeSlider.onValueChanged
                .AddListener(OnKeyboardMouseDragRangeChanged);

            keyboardMouseSlingshotToggle.onValueChanged
                .AddListener(OnKeyboardMouseSlingshotChanged);

            gamepadSlingshotToggle.onValueChanged
                .AddListener(OnGamepadSlingshotChanged);


            masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
            masterMutedToggle.onValueChanged.AddListener(OnMasterMutedChanged);
            bgmVolumeSlider.onValueChanged.AddListener(OnBgmVolumeChanged);
            bgmMutedToggle.onValueChanged.AddListener(OnBgmMutedChanged);
            sfxVolumeSlider.onValueChanged.AddListener(OnSfxVolumeChanged);
            sfxMutedToggle.onValueChanged.AddListener(OnSfxMutedChanged);

            PopulateLanguageDropdown();
            languageDropdown.onValueChanged.AddListener(OnLanguageChanged);

            ShowKeyboardMouseTab();
        }

        private void OnDestroy()
        {
            keyboardMouseTabButton.onClick.RemoveListener(ShowKeyboardMouseTab);
            gamepadTabButton.onClick.RemoveListener(ShowGamepadTab);
            keyboardMouseDragRangeSlider.onValueChanged.RemoveListener(OnKeyboardMouseDragRangeChanged);
            keyboardMouseSlingshotToggle.onValueChanged.RemoveListener(OnKeyboardMouseSlingshotChanged);
            gamepadSlingshotToggle.onValueChanged.RemoveListener(OnGamepadSlingshotChanged);

            masterVolumeSlider.onValueChanged.RemoveListener(OnMasterVolumeChanged);
            masterMutedToggle.onValueChanged.RemoveListener(OnMasterMutedChanged);
            bgmVolumeSlider.onValueChanged.RemoveListener(OnBgmVolumeChanged);
            bgmMutedToggle.onValueChanged.RemoveListener(OnBgmMutedChanged);
            sfxVolumeSlider.onValueChanged.RemoveListener(OnSfxVolumeChanged);
            sfxMutedToggle.onValueChanged.RemoveListener(OnSfxMutedChanged);

            languageDropdown.onValueChanged.RemoveListener(OnLanguageChanged);
        }

        public void Bind(SettingsData draft)
        {
            _draft = draft;

            var keyboardMouse = _draft.inputProfile.keyboardMouse;
            var gamepad = _draft.inputProfile.gamepad;

            keyboardMouseDragRangeSlider.SetValueWithoutNotify(keyboardMouse.drag.dragRange);
            keyboardMouseSlingshotToggle.SetIsOnWithoutNotify(keyboardMouse.drag.isSlingshot);

            gamepadSlingshotToggle.SetIsOnWithoutNotify(gamepad.direct.isSlingshot);

            var volume = _draft.audioSettings.volume;
            masterVolumeSlider.SetValueWithoutNotify(volume.master.volume);
            masterMutedToggle.SetIsOnWithoutNotify(volume.master.muted);

            bgmVolumeSlider.SetValueWithoutNotify(volume.bgm.volume);
            bgmMutedToggle.SetIsOnWithoutNotify(volume.bgm.muted);

            sfxVolumeSlider.SetValueWithoutNotify(volume.sfx.volume);
            sfxMutedToggle.SetIsOnWithoutNotify(volume.sfx.muted);

            var localeCode = _draft.localizationSettings.localeCode ?? LocalizationManager.Instance.CurrentLocaleCode;
            var localeIndex =
                _locales.FindIndex(locale => locale.Identifier.Code == localeCode);
            languageDropdown.SetValueWithoutNotify(localeIndex);
            languageDropdown.RefreshShownValue();
        }

        public void Unbind()
        {
            _draft = null;
        }

        private void ShowKeyboardMouseTab()
        {
            keyboardMousePanel.SetActive(true);
            gamepadPanel.SetActive(false);

            keyboardMouseTabButton.interactable = false;
            gamepadTabButton.interactable = true;
        }

        private void ShowGamepadTab()
        {
            keyboardMousePanel.SetActive(false);
            gamepadPanel.SetActive(true);

            keyboardMouseTabButton.interactable = true;
            gamepadTabButton.interactable = false;
        }

        private void OnKeyboardMouseDragRangeChanged(float value)
        {
            _draft.inputProfile.keyboardMouse.drag.dragRange = value;
        }

        private void OnKeyboardMouseSlingshotChanged(bool value)
        {
            _draft.inputProfile.keyboardMouse.drag.isSlingshot = value;
        }

        private void OnGamepadSlingshotChanged(bool value)
        {
            _draft.inputProfile.gamepad.direct.isSlingshot = value;
        }

        private void OnMasterVolumeChanged(float value)
        {
            _draft.audioSettings.volume.master.volume = value;
            AudioManager.Instance.SetMasterVolume(value);
        }

        private void OnMasterMutedChanged(bool value)
        {
            _draft.audioSettings.volume.master.muted = value;
            AudioManager.Instance.SetMasterMuted(value);
        }

        private void OnBgmVolumeChanged(float value)
        {
            _draft.audioSettings.volume.bgm.volume = value;
            AudioManager.Instance.SetBgmVolume(value);
        }

        private void OnBgmMutedChanged(bool value)
        {
            _draft.audioSettings.volume.bgm.muted = value;
            AudioManager.Instance.SetBgmMuted(value);
        }

        private void OnSfxVolumeChanged(float value)
        {
            _draft.audioSettings.volume.sfx.volume = value;
            AudioManager.Instance.SetSfxVolume(value);
        }

        private void OnSfxMutedChanged(bool value)
        {
            _draft.audioSettings.volume.sfx.muted = value;
            AudioManager.Instance.SetSfxMuted(value);
        }

        private void PopulateLanguageDropdown()
        {
            _locales.Clear();
            _locales.AddRange(LocalizationManager.Instance.AvailableLocales
                .Where(locale => locale is not PseudoLocale)
                .OrderBy(locale => locale.SortOrder));
            var options = _locales.Select(locale =>
            {
                var presentation = locale.Metadata.GetMetadata<LocalePresentationMetadata>();
                return new TMP_Dropdown.OptionData(presentation?.Icon);
            }).ToList();

            languageDropdown.ClearOptions();
            languageDropdown.AddOptions(options);
        }

        private void OnLanguageChanged(int index)
        {
            var locale = _locales[index];
            _draft.localizationSettings.localeCode = locale.Identifier.Code;
            LocalizationManager.Instance.CurrentLocale = locale;
        }
    }
}