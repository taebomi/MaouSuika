using TBM.MaouSuika.Core.Settings;

namespace TBM.MaouSuika.Core.Save
{
    public interface ISettingsDataHandler
    {
        void OnSaveData(SettingsData data);
        void OnLoadData(SettingsData data);
    }
}