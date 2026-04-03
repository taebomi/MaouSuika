using SOSG.System.Setting;
using TaeBoMi;
using UnityEngine;

namespace SOSG.System.Display
{
    public class DisplayManager : MonoBehaviour
    {
        [SerializeField] private PillarBoxController pillarBoxController;

        public void SetUp()
        {
            pillarBoxController.SetUp();
            ApplySetting(SettingDataHelper.DisplaySetting);
            SettingDataHelper.DisplaySettingChanged += ApplySetting;
        }

        public void TearDown()
        {
            SettingDataHelper.DisplaySettingChanged -= ApplySetting;
            pillarBoxController.TearDown();
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus && DisplayData.IsScreenChanged())
            {
                TBMUtility.Log($"# DisplayManager - OnApplicationPause - Screen Changed");
                DisplayData.Update();
            }
        }

        private void ApplySetting(DisplaySetting displaySetting)
        {
            ChangeResolution(displaySetting.resolution);
            ChangeFps(displaySetting.fps);
        }

        private void ChangeResolution(DisplaySetting.Resolution resolution)
        {
            var resolutionHeight = resolution switch
            {
                DisplaySetting.Resolution.Low => 720,
                DisplaySetting.Resolution.Mid => 1080,
                DisplaySetting.Resolution.High => 1440,
                _ => 1080
            };
            Screen.SetResolution((int)(resolutionHeight / DisplayData.ScreenRatio), resolutionHeight, true);
        }

        private void ChangeFps(DisplaySetting.FPS fps)
        {
            Application.targetFrameRate = fps switch
            {
                DisplaySetting.FPS.Low => 30,
                DisplaySetting.FPS.Mid => 45,
                DisplaySetting.FPS.High => 60,
                _ => 60
            };
        }
    }
}