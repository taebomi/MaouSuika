using System;
using UnityEngine;

namespace SOSG.System.Setting
{
    public static class SettingDataHelper
    {
        public static AudioSetting AudioSetting { get; set; }
        public static VibrationSetting VibrationSetting { get; set; }
        public static DisplaySetting DisplaySetting { get; set; }
        public static InterfaceSetting InterfaceSetting { get; set; }
        public static ControlSetting ControlSetting { get; set; }

        public static event Action<AudioSetting> AudioSettingChanged;
        public static event Action<VibrationSetting> VibrationSettingChanged;
        public static event Action<DisplaySetting> DisplaySettingChanged;
        public static event Action<InterfaceSetting> InterfaceSettingChanged;
        public static event Action<ControlSetting> ControlSettingChanged;

        public static event Action SaveRequested;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void InitializeOnSubsystemRegistration()
        {
            AudioSetting = new AudioSetting();
            VibrationSetting = new VibrationSetting();
            DisplaySetting = new DisplaySetting();
            InterfaceSetting = new InterfaceSetting();
            ControlSetting = new ControlSetting();

            AudioSettingChanged = null;
            VibrationSettingChanged = null;
            DisplaySettingChanged = null;
            InterfaceSettingChanged = null;
            ControlSettingChanged = null;
            SaveRequested = null;
        }

        public static void ApplySetting(DisplaySetting displaySetting)
        {
            if (DisplaySetting.IsEqual(displaySetting))
            {
                return;
            }

            DisplaySetting = new DisplaySetting(displaySetting);
            DisplaySettingChanged?.Invoke(DisplaySetting);
        }
        
        public static void ApplySetting(AudioSetting audioSetting)
        {
            if (AudioSetting.IsEqual(audioSetting))
            {
                return;
            }

            AudioSetting = new AudioSetting(audioSetting);
            AudioSettingChanged?.Invoke(AudioSetting);
        }
        
        public static void ApplySetting(VibrationSetting vibrationSetting)
        {
            if (VibrationSetting.IsEqual(vibrationSetting))
            {
                return;
            }

            VibrationSetting = new VibrationSetting(vibrationSetting);
            VibrationSettingChanged?.Invoke(VibrationSetting);
        }

        public static void ApplySetting(ControlSetting controlSetting)
        {
            if (ControlSetting.IsEqual(controlSetting))
            {
                return;
            }

            ControlSetting = new ControlSetting(controlSetting);
            ControlSettingChanged?.Invoke(ControlSetting);
        }

        public static void ApplySetting(InterfaceSetting interfaceSetting)
        {
            if (InterfaceSetting.IsEqual(interfaceSetting))
            {
                return;
            }

            InterfaceSetting = new InterfaceSetting(interfaceSetting);
            InterfaceSettingChanged?.Invoke(InterfaceSetting);
        }

        public static bool IsEqual(AudioSetting audioSetting)
        {
            return AudioSetting.IsEqual(audioSetting);
        }
        
        public static bool IsEqual(DisplaySetting displaySetting)
        {
            return DisplaySetting.IsEqual(displaySetting);
        }
        
        public static bool IsEqual(InterfaceSetting interfaceSetting)
        {
            return InterfaceSetting.IsEqual(interfaceSetting);
        }
        
        public static bool IsEqual(VibrationSetting vibrationSetting)
        {
            return VibrationSetting.IsEqual(vibrationSetting);
        }
        
        public static bool IsEqual(ControlSetting controlSetting)
        {
            return ControlSetting.IsEqual(controlSetting);
        }

        public static void RequestSave() => SaveRequested?.Invoke();
    }
}