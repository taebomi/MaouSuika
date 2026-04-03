using TBM.MaouSuika.Core.Save;
using TBM.MaouSuika.Core.Settings;
using UnityEngine;

namespace TBM.MaouSuika.Core.Vibration
{
    public class VibrationManager : CoreManager<VibrationManager>, ISettingsDataHandler
    {
        private const float VIBRATION_INTERVAL = 0.1f;

        private bool _isVibrationEnabled;
        private float _vibrationIntensity;

        private float _nextVibrationAvailableTime;

        public void Enable()
        {
            _isVibrationEnabled = true;
        }

        public void Disable()
        {
            _isVibrationEnabled = false;
        }


        public void OnSaveData(SettingsData data)
        {
        }

        public void OnLoadData(SettingsData data)
        {
        }

        public void Play(VibrationType type)
        {
            if (!_isVibrationEnabled) return;
            if (Time.unscaledTime < _nextVibrationAvailableTime) return;

            _nextVibrationAvailableTime = Time.unscaledTime + VIBRATION_INTERVAL;
            
            // todo Vibration
        }
    }
}