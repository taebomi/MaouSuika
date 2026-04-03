using System.Collections.Generic;
using TBM.MaouSuika.Core.Settings;

namespace TBM.MaouSuika.Core.Save
{
    public class SaveManager : CoreManager<SaveManager>
    {
        private SettingsStorage _settingsStorage;

        private readonly List<ISettingsDataHandler> _settingsHandlers = new();

        public SettingsData SettingsData { get; private set; } = new();

        private void Awake()
        {
            _settingsStorage = new SettingsStorage();
        }


        public void SaveSettingsData()
        {
            foreach (var settingsHandler in _settingsHandlers)
            {
                settingsHandler.OnSaveData(SettingsData);
            }

            _settingsStorage.Save(SettingsData);
        }

        public SettingsData LoadSettingsData()
        {
            SettingsData = _settingsStorage.Load();
            return SettingsData;
        }

        public void RegisterSettingsHandler(ISettingsDataHandler settingsHandler)
        {
            _settingsHandlers.Add(settingsHandler);
        }
    }
}