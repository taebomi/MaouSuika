using System;
using SOSG.System.Setting;
using UnityEngine;

namespace SOSG.System.Audio
{
    public class AudioManager : MonoBehaviour
    {
        private readonly BgmController _bgmController = new();
        private readonly SfxController _sfxController = new();
        private readonly OverlordSfxController _overlordSfxController = new();

        private void OnEnable()
        {
            SettingDataHelper.AudioSettingChanged += ApplySetting;

            AudioSystemHelper.BgmPlayRequested += _bgmController.Play;
            AudioSystemHelper.BgmWithMarkerPlayRequested += _bgmController.Play;
            AudioSystemHelper.BgmPitchChangeRequested += _bgmController.SetPitch;
            AudioSystemHelper.BgmTimelinePositionChangeRequested += _bgmController.SetTime;

            AudioSystemHelper.SfxPlayRequested += _sfxController.Play;
            AudioSystemHelper.SfxWithPitchPlayRequested += _sfxController.Play;

            AudioSystemHelper.OverlordSfxPlayRequested += _overlordSfxController.Play;
        }

        private void OnDisable()
        {
            SettingDataHelper.AudioSettingChanged -= ApplySetting;

            AudioSystemHelper.BgmPlayRequested -= _bgmController.Play;
            AudioSystemHelper.BgmWithMarkerPlayRequested -= _bgmController.Play;
            AudioSystemHelper.BgmPitchChangeRequested -= _bgmController.SetPitch;
            AudioSystemHelper.BgmTimelinePositionChangeRequested -= _bgmController.SetTime;

            AudioSystemHelper.SfxPlayRequested -= _sfxController.Play;
            AudioSystemHelper.SfxWithPitchPlayRequested -= _sfxController.Play;

            AudioSystemHelper.OverlordSfxPlayRequested -= _overlordSfxController.Play;

            _bgmController.Stop();
        }

        public void ApplySetting(AudioSetting audioSetting)
        {
            _bgmController.SetVolume(audioSetting.bgmMute ? 0f : audioSetting.bgmVolume);
            _sfxController.SetVolume(audioSetting.sfxMute ? 0f : audioSetting.sfxVolume);
            _overlordSfxController.SetVolume(audioSetting.overlordSfxMute ? 0f : audioSetting.overlordSfxVolume);
        }
    }
}