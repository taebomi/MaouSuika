using UnityEngine;

namespace MaouSuika.Core.Save
{
    public class SaveManager : MonoBehaviour
    {
        public static SaveManager Instance { get; private set; }

        private SettingsStorage _settingsStorage;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            Instance = null;
        }

        public void Initialize()
        {
            Instance = this;

            _settingsStorage = new SettingsStorage();
        }

        private void OnDestroy()
        {
            if (!ReferenceEquals(Instance, this)) return;

            Instance = null;
        }

        public void SaveSettings(SettingsData settings)
        {
            _settingsStorage.Save(settings);
        }

        public SettingsData LoadSettings()
        {
            var data = _settingsStorage.LoadOrDefault();
            data.Normalize();
            return data;
        }
    }
}