using UnityEngine;

namespace SOSG.System.Setting
{
    public class SettingDataManager : MonoBehaviour
    {
        private ES3Settings _es3Settings;
        private const string FilePath = "SOSG.setting";

        public void SetUp()
        {
            SetUpCallbacks();

            Load();
        }

        public void TearDown()
        {
            TearDownCallbacks();
        }

        private void SetUpCallbacks()
        {
            SettingDataHelper.SaveRequested += Save;
        }

        private void TearDownCallbacks()
        {
            SettingDataHelper.SaveRequested -= Save;
        }

        private void Load()
        {
            try
            {
                _es3Settings = new ES3Settings(ES3.Location.Cache, ES3.CompressionType.None, ES3.EncryptionType.None);
                _es3Settings = new ES3Settings(FilePath, _es3Settings);
                ES3.CacheFile(_es3Settings);

                SettingDataHelper.AudioSetting = ES3.Load("Audio", new AudioSetting(), _es3Settings);
                SettingDataHelper.VibrationSetting = ES3.Load("Vibration", new VibrationSetting(), _es3Settings);
                SettingDataHelper.ControlSetting = ES3.Load("Control", new ControlSetting(), _es3Settings);
                SettingDataHelper.DisplaySetting = ES3.Load("Display", new DisplaySetting(), _es3Settings);
                SettingDataHelper.InterfaceSetting = ES3.Load("Interface", new InterfaceSetting(), _es3Settings);
            }
            catch
            {
                Debug.LogError($"# SettingDataManager - Load - Failed to load setting data.");
                SettingDataHelper.AudioSetting = new AudioSetting();
                SettingDataHelper.VibrationSetting = new VibrationSetting();
                SettingDataHelper.ControlSetting = new ControlSetting();
                SettingDataHelper.DisplaySetting = new DisplaySetting();
                SettingDataHelper.InterfaceSetting = new InterfaceSetting();
            }
        }

        private void Save()
        {
            try
            {
                ES3.Save("Audio", SettingDataHelper.AudioSetting, _es3Settings);
                ES3.Save("Vibration", SettingDataHelper.VibrationSetting, _es3Settings);
                ES3.Save("Control", SettingDataHelper.ControlSetting, _es3Settings);
                ES3.Save("Display", SettingDataHelper.DisplaySetting, _es3Settings);
                ES3.Save("Interface", SettingDataHelper.InterfaceSetting, _es3Settings);
                ES3.StoreCachedFile(_es3Settings);
            }
            catch
            {
                Debug.LogError($"# SettingDataManager - Save - Failed to save setting data.");
            }
        }
    }
}