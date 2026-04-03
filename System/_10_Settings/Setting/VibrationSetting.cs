using System;

namespace SOSG.System.Setting
{
    [Serializable]
    [ES3Serializable]
    public class VibrationSetting
    {
        public bool isEnabled;

        public VibrationSetting()
        {
            isEnabled = true;
        }

        public VibrationSetting(VibrationSetting vibrationSetting)
        {
            isEnabled = vibrationSetting.isEnabled;
        }

        public void Set(bool enabled)
        {
            isEnabled = enabled;
        }

        public bool IsEqual(VibrationSetting vibrationSetting)
        {
            return isEnabled == vibrationSetting.isEnabled;
        }
    }
}