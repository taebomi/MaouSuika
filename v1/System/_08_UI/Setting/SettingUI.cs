using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using SOSG.System.Localization;
using SOSG.System.Scene;
using SOSG.System.Setting;
using SOSG.System.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace SOSG.UI
{
    public class SettingUI : BaseUI, IModalUI
    {
        [SerializeField] private Scrollbar scrollbar;

        #region UI Components

        [Header("Elements")]
        // Display
        [SerializeField] private Button resolutionLeftBtn;
        [SerializeField] private TMP_Text resolutionCurTmp;
        [SerializeField] private Button resolutionRightBtn;

        [SerializeField] private Button fpsLeftBtn;
        [SerializeField] private TMP_Text fpsCurTmp;
        [SerializeField] private Button fpsRightBtn;

        // audio
        [SerializeField] private Toggle bgmToggle;
        [SerializeField] private Slider bgmSlider;
        [SerializeField] private Toggle sfxToggle;
        [SerializeField] private Slider sfxSlider;
        [SerializeField] private Toggle overlordSfxToggle;
        [SerializeField] private Slider overlordSfxSlider;

        // vibration
        [SerializeField] private Toggle vibrationToggle;

        // control
        [SerializeField] private Toggle dragInverseToggle;
        [SerializeField] private Slider dragRangeSlider;

        // interface
        [SerializeField] private Toggle suikaMergeGuideToggle;

        #endregion

        private DisplaySetting _displaySetting;
        private AudioSetting _audioSetting;
        private VibrationSetting _vibrationSetting;
        private ControlSetting _controlSetting;
        private InterfaceSetting _interfaceSetting;

        private Selectable _lastSelected;

        private TBMStringTable _tbmStringTable;

        public Action DeactivateEvent;

        private bool _isShowing;

        protected override void AwakeAfter()
        {
            Canvas.enabled = false;
            SceneSetUpHelper.AddTask(SetUpAsync);
        }

        private async UniTask SetUpAsync()
        {
            _tbmStringTable = GetComponent<TBMStringTable>();
            await _tbmStringTable.SetUpAsync(LocalizationTableName.Setting);
        }


        public void Show()
        {
            GetAndShowCurrentSetting();
            _tbmStringTable.PrintConversation("enter");
            UIHelper.ShowUI(this);
            Canvas.enabled = true;
            scrollbar.value = 1f;
            ShowAsync().Forget();
        }

        private async UniTaskVoid ShowAsync()
        {
            await transform.SOSGUIPopUp().Play().WithCancellation(destroyCancellationToken);
            _isShowing = true;
        }

        public void Hide()
        {
            if (_isShowing is false)
            {
                return;
            }

            Canvas.enabled = false;
            UIHelper.HideUI(this);
            DeactivateEvent?.Invoke();
            _isShowing = false;
        }

        public void OnOverlayClicked()
        {
            Hide();
        }

        public void OnCloseRequested()
        {
            Hide();
        }

        public void OnSaveBtnClicked()
        {
            if (IsSettingChanged())
            {
                ApplyAndSaveSetting();
                _tbmStringTable.PrintConversation("save");
                Hide();
            }
            else
            {
                _tbmStringTable.PrintConversation("save_but_nothing_changed");
                Hide();
            }
        }

        public void OnCancelRequested()
        {
            if (IsSettingChanged())
            {
                AskQuitWithoutSaveSettings().Forget();
            }
            else
            {
                _tbmStringTable.PrintConversation("quit_without_changing");
                Hide();
            }
        }

        private async UniTask AskQuitWithoutSaveSettings()
        {
            var answer = await _tbmStringTable.ShowChoiceAsync("ask_quit_without_saving", "choice_quit");
            if (answer is 0)
            {
                _tbmStringTable.PrintConversation("quit_without_saving");
                Hide();
            }
            else
            {
                _tbmStringTable.PrintConversation("save_please");
            }
        }

        private void GetAndShowCurrentSetting()
        {
            _displaySetting = new DisplaySetting(SettingDataHelper.DisplaySetting);
            // Display
            switch (_displaySetting.resolution)
            {
                case DisplaySetting.Resolution.Low:
                    resolutionLeftBtn.gameObject.SetActive(false);
                    resolutionCurTmp.text = _tbmStringTable.GetLocalizedString("resolution_low");
                    resolutionRightBtn.gameObject.SetActive(true);
                    break;
                case DisplaySetting.Resolution.Mid:
                    resolutionLeftBtn.gameObject.SetActive(true);
                    resolutionCurTmp.text = _tbmStringTable.GetLocalizedString("resolution_mid");
                    resolutionRightBtn.gameObject.SetActive(true);
                    break;
                case DisplaySetting.Resolution.High:
                    resolutionLeftBtn.gameObject.SetActive(true);
                    resolutionCurTmp.text = _tbmStringTable.GetLocalizedString("resolution_high");
                    resolutionRightBtn.gameObject.SetActive(false);
                    break;
            }

            switch (_displaySetting.fps)
            {
                case DisplaySetting.FPS.Low:
                    fpsLeftBtn.gameObject.SetActive(false);
                    fpsCurTmp.text = _tbmStringTable.GetLocalizedString("fps_low");
                    fpsRightBtn.gameObject.SetActive(true);
                    break;
                case DisplaySetting.FPS.Mid:
                    fpsLeftBtn.gameObject.SetActive(true);
                    fpsCurTmp.text = _tbmStringTable.GetLocalizedString("fps_mid");
                    fpsRightBtn.gameObject.SetActive(true);
                    break;
                case DisplaySetting.FPS.High:
                    fpsLeftBtn.gameObject.SetActive(true);
                    fpsCurTmp.text = _tbmStringTable.GetLocalizedString("fps_high");
                    fpsRightBtn.gameObject.SetActive(false);
                    break;
            }

            // Audio
            _audioSetting = new AudioSetting(SettingDataHelper.AudioSetting);

            bgmToggle.SetIsOnWithoutNotify(!_audioSetting.bgmMute);

            bgmSlider.interactable = !_audioSetting.bgmMute;
            bgmSlider.SetValueWithoutNotify(_audioSetting.bgmMute ? 0f : _audioSetting.bgmVolume);

            sfxToggle.SetIsOnWithoutNotify(!_audioSetting.sfxMute);
            sfxSlider.interactable = !_audioSetting.sfxMute;
            sfxSlider.SetValueWithoutNotify(_audioSetting.sfxMute ? 0f : _audioSetting.sfxVolume);

            overlordSfxToggle.SetIsOnWithoutNotify(!_audioSetting.overlordSfxMute);
            overlordSfxSlider.interactable = !_audioSetting.overlordSfxMute;
            overlordSfxSlider.SetValueWithoutNotify(
                _audioSetting.overlordSfxMute ? 0f : _audioSetting.overlordSfxVolume);

            // Vibration
            _vibrationSetting = new VibrationSetting(SettingDataHelper.VibrationSetting);
            vibrationToggle.SetIsOnWithoutNotify(_vibrationSetting.isEnabled);

            // Control
            _controlSetting = new ControlSetting(SettingDataHelper.ControlSetting);
            dragInverseToggle.SetIsOnWithoutNotify(_controlSetting.invertYAxis);
            dragRangeSlider.SetValueWithoutNotify((_controlSetting.dragRange - ControlSetting.MinDragRange) /
                                                  (ControlSetting.MaxDragRange - ControlSetting.MinDragRange));
            // Interface
            _interfaceSetting = new InterfaceSetting(SettingDataHelper.InterfaceSetting);
            suikaMergeGuideToggle.SetIsOnWithoutNotify(_interfaceSetting.suikaMergeGuide);
        }

        private bool IsSettingChanged()
        {
            var allEqual = SettingDataHelper.IsEqual(_audioSetting) &&
                           SettingDataHelper.IsEqual(_displaySetting) &&
                           SettingDataHelper.IsEqual(_vibrationSetting) &&
                           SettingDataHelper.IsEqual(_controlSetting) &&
                           SettingDataHelper.IsEqual(_interfaceSetting);

            return !allEqual;
        }

        private void ApplyAndSaveSetting()
        {
            SettingDataHelper.ApplySetting(_displaySetting);
            SettingDataHelper.ApplySetting(_audioSetting);
            SettingDataHelper.ApplySetting(_vibrationSetting);
            SettingDataHelper.ApplySetting(_controlSetting);
            SettingDataHelper.ApplySetting(_interfaceSetting);
            SettingDataHelper.RequestSave();
        }

        #region UI Component Callbacks

        #region Display

        public void OnResolutionLeftBtnClicked()
        {
            _displaySetting.resolution = _displaySetting.resolution switch
            {
                DisplaySetting.Resolution.Low => DisplaySetting.Resolution.Low,
                DisplaySetting.Resolution.Mid => DisplaySetting.Resolution.Low,
                DisplaySetting.Resolution.High => DisplaySetting.Resolution.Mid,
                _ => throw new ArgumentOutOfRangeException()
            };

            OnResolutionSettingChanged();
            _lastSelected = resolutionLeftBtn.IsActive() ? resolutionLeftBtn : resolutionRightBtn;
        }

        public void OnResolutionRightBtnClicked()
        {
            _displaySetting.resolution = _displaySetting.resolution switch
            {
                DisplaySetting.Resolution.Low => DisplaySetting.Resolution.Mid,
                DisplaySetting.Resolution.Mid => DisplaySetting.Resolution.High,
                DisplaySetting.Resolution.High => DisplaySetting.Resolution.High,
                _ => throw new ArgumentOutOfRangeException()
            };

            OnResolutionSettingChanged();
            _lastSelected = resolutionRightBtn.IsActive() ? resolutionRightBtn : resolutionLeftBtn;
        }

        private void OnResolutionSettingChanged()
        {
            switch (_displaySetting.resolution)
            {
                case DisplaySetting.Resolution.Low:
                    resolutionLeftBtn.gameObject.SetActive(false);
                    resolutionCurTmp.text = _tbmStringTable.GetLocalizedString("resolution_low");
                    resolutionRightBtn.gameObject.SetActive(true);
                    _tbmStringTable.PrintConversation("resolution_low_set");
                    break;
                case DisplaySetting.Resolution.Mid:
                    resolutionLeftBtn.gameObject.SetActive(true);
                    resolutionCurTmp.text = _tbmStringTable.GetLocalizedString("resolution_mid");
                    resolutionRightBtn.gameObject.SetActive(true);
                    _tbmStringTable.PrintConversation("resolution_mid_set");
                    break;
                case DisplaySetting.Resolution.High:
                    resolutionLeftBtn.gameObject.SetActive(true);
                    resolutionCurTmp.text = _tbmStringTable.GetLocalizedString("resolution_high");
                    resolutionRightBtn.gameObject.SetActive(false);
                    _tbmStringTable.PrintConversation("resolution_high_set");
                    break;
            }
        }

        public void OnFpsLeftBtnClicked()
        {
            _displaySetting.fps = _displaySetting.fps switch
            {
                DisplaySetting.FPS.Low => DisplaySetting.FPS.Low,
                DisplaySetting.FPS.Mid => DisplaySetting.FPS.Low,
                DisplaySetting.FPS.High => DisplaySetting.FPS.Mid,
                _ => throw new ArgumentOutOfRangeException()
            };

            OnFpsSettingChanged();
            _lastSelected = fpsLeftBtn.IsActive() ? fpsLeftBtn : fpsRightBtn;
        }

        public void OnFpsRightBtnClicked()
        {
            _displaySetting.fps = _displaySetting.fps switch
            {
                DisplaySetting.FPS.Low => DisplaySetting.FPS.Mid,
                DisplaySetting.FPS.Mid => DisplaySetting.FPS.High,
                DisplaySetting.FPS.High => DisplaySetting.FPS.High,
                _ => throw new ArgumentOutOfRangeException()
            };

            OnFpsSettingChanged();
            _lastSelected = fpsRightBtn.IsActive() ? fpsRightBtn : fpsLeftBtn;
        }

        private void OnFpsSettingChanged()
        {
            switch (_displaySetting.fps)
            {
                case DisplaySetting.FPS.Low:
                    fpsLeftBtn.gameObject.SetActive(false);
                    fpsCurTmp.text = _tbmStringTable.GetLocalizedString("fps_low");
                    _tbmStringTable.PrintConversation("fps_low_set");
                    fpsRightBtn.gameObject.SetActive(true);
                    break;
                case DisplaySetting.FPS.Mid:
                    fpsLeftBtn.gameObject.SetActive(true);
                    fpsCurTmp.text = _tbmStringTable.GetLocalizedString("fps_mid");
                    _tbmStringTable.PrintConversation("fps_mid_set");
                    fpsRightBtn.gameObject.SetActive(true);
                    break;
                case DisplaySetting.FPS.High:
                    fpsLeftBtn.gameObject.SetActive(true);
                    fpsCurTmp.text = _tbmStringTable.GetLocalizedString("fps_high");
                    _tbmStringTable.PrintConversation("fps_high_set");
                    fpsRightBtn.gameObject.SetActive(false);
                    break;
            }
        }

        #endregion

        #region Audio

        public void OnBgmToggled(bool value)
        {
            _audioSetting.SetBgmMute(!value);
            _tbmStringTable.PrintConversation(value ? "bgm_on" : "bgm_off");
            _lastSelected = bgmToggle;
            bgmSlider.interactable = !_audioSetting.bgmMute;
            bgmSlider.SetValueWithoutNotify(_audioSetting.bgmMute ? 0f : _audioSetting.bgmVolume);
        }

        public void OnBgmVolumeChanged(float value)
        {
            if (_audioSetting.bgmMute)
            {
                return;
            }

            _audioSetting.SetBgmVolume(value);
            if (_lastSelected != bgmSlider)
            {
                _tbmStringTable.PrintConversation("bgm_volume_changed");
            }

            _lastSelected = bgmSlider;
        }

        public void OnSfxToggled(bool value)
        {
            _audioSetting.SetSfxMute(!value);
            _tbmStringTable.PrintConversation(value ? "sfx_on" : "sfx_off");
            _lastSelected = sfxToggle;
            sfxSlider.interactable = !_audioSetting.sfxMute;
            sfxSlider.SetValueWithoutNotify(_audioSetting.sfxMute ? 0f : _audioSetting.sfxVolume);
        }

        public void OnSfxVolumeChanged(float value)
        {
            if (_audioSetting.sfxMute)
            {
                return;
            }

            _audioSetting.SetSfxVolume(value);
            if (_lastSelected != sfxSlider)
            {
                _tbmStringTable.PrintConversation("sfx_volume_changed");
            }

            _lastSelected = sfxSlider;
        }

        public void OnOverlordSfxToggled(bool value)
        {
            _audioSetting.SetOverlordSfxMute(!value);
            _tbmStringTable.PrintConversation(value ? "overlord_sfx_on" : "overlord_sfx_off");
            overlordSfxSlider.interactable = !_audioSetting.overlordSfxMute;
            overlordSfxSlider.SetValueWithoutNotify(
                _audioSetting.overlordSfxMute ? 0f : _audioSetting.overlordSfxVolume);

            _lastSelected = overlordSfxToggle;
        }

        public void OnOverlordSfxVolumeChanged(float value)
        {
            if (_audioSetting.overlordSfxMute)
            {
                return;
            }

            _audioSetting.SetOverlordSfxVolume(value);
            if (_lastSelected != overlordSfxSlider)
            {
                _tbmStringTable.PrintConversation("overlord_sfx_volume_changed");
            }

            _lastSelected = overlordSfxSlider;
        }

        #endregion

        #region Vibration

        public void OnVibrationToggled(bool value)
        {
            _vibrationSetting.Set(value);
            _tbmStringTable.PrintConversation(value ? "vibration_on" : "vibration_off");
            _lastSelected = vibrationToggle;
        }

        #endregion

        #region Control

        public void OnInvertYAxisToggled(bool value)
        {
            _controlSetting.SetDragInverse(value);
            _tbmStringTable.PrintConversation(value ? "invert_y_axis_on" : "invert_y_axis_off");
            _lastSelected = dragInverseToggle;
        }

        public void OnDragRangeChanged(float value)
        {
            _controlSetting.SetDragRange(value);
            if (_lastSelected != dragRangeSlider)
            {
                _tbmStringTable.PrintConversation("drag_range_changed");
            }

            _lastSelected = dragRangeSlider;
        }

        #endregion

        #region Interface

        public void OnSuikaMergeGuideToggled(bool value)
        {
            _interfaceSetting.SetSuikaMergeGuide(value);
            _tbmStringTable.PrintConversation(value ? "suika_tier_guide_on" : "suika_tier_guide_off");
            _lastSelected = suikaMergeGuideToggle;
        }

        #endregion

        #endregion
    }
}