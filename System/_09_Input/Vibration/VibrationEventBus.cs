using System;
using Lofelt.NiceVibrations;
using UnityEngine;

namespace SOSG.System.Vibration
{
    public static class VibrationEventBus
    {
        public static event Action<HapticPatterns.PresetType> PresetPlayRequested;
        public static event Action<float, float, float> ConstantPlayRequested;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void InitializeOnSubsystemRegistration()
        {
            PresetPlayRequested = null;
            ConstantPlayRequested = null;
        }

        public static void PlayPreset(HapticPatterns.PresetType presetType)
        {
            PresetPlayRequested?.Invoke(presetType);
        }

        public static void PlayConstant(float amplitude, float frequency, float duration)
        {
            ConstantPlayRequested?.Invoke(amplitude, frequency, duration);
        }
    }
}