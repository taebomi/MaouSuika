using System;
using UnityEngine;

namespace MaouSuika.Core
{
    public class SettingsManager : MonoBehaviour
    {
        public static SettingsManager Instance { get; private set; }

        [SerializeField] private InputManager input;
        [SerializeField] private AudioManager audioManager;
        [SerializeField] private LocalizationManager localizationManager;


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            Instance = null;
        }

        public void Initialize()
        {
            Instance = this;
        }

        private void OnDestroy()
        {
            if (ReferenceEquals(Instance, this) is false) return;

            Instance = null;
        }

        public void Apply(SettingsData data)
        {
            input.MainPlayer.ApplyInputProfile(data.inputProfile);
            audioManager.ApplySettings(data.audioSettings);
            localizationManager.ApplySettings(data.localizationSettings);
        }

        public SettingsData Capture()
        {
            return new SettingsData
            {
                inputProfile = input.CaptureSettings(),
                audioSettings = audioManager.CaptureSettings(),
                localizationSettings = localizationManager.CaptureSettings(),
            };
        }
    }
}
