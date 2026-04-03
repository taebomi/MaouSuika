using System;
using Lofelt.NiceVibrations;
using SOSG.System.Setting;
using UnityEngine;

namespace SOSG.System.Vibration
{
    public class VibrationManager : MonoBehaviour
    {
        private bool _isEnabled;

        public void SetUp()
        {
            SettingDataHelper.VibrationSettingChanged += ApplySetting;

            VibrationEventBus.PresetPlayRequested += PlayPatternPreset;
            VibrationEventBus.ConstantPlayRequested += PlayPatternConstant;

            ApplySetting(SettingDataHelper.VibrationSetting);
        }

        public void TearDown()
        {
            SettingDataHelper.VibrationSettingChanged -= ApplySetting;

            VibrationEventBus.PresetPlayRequested -= PlayPatternPreset;
            VibrationEventBus.ConstantPlayRequested -= PlayPatternConstant;
        }

        private void ApplySetting(VibrationSetting setting)
        {
            _isEnabled = setting.isEnabled;
        }

        private void PlayPatternPreset(HapticPatterns.PresetType presetType)
        {
            if (_isEnabled is false)
            {
                return;
            }

            HapticPatterns.PlayPreset(presetType);
        }

        private void PlayPatternConstant(float amplitude, float frequency, float duration)
        {
            if (_isEnabled is false)
            {
                return;
            }

            HapticPatterns.PlayConstant(amplitude, frequency, duration);
        }
    }
}