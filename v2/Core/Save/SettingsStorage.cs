using TBM.MaouSuika.Core.Settings;

namespace TBM.MaouSuika.Core.Save
{
    public class SettingsStorage
    {
        private const string FILE_PATH = "Settings.json";
        private const string KEY = "Settings";

        private readonly ES3Settings _es3Settings = new()
        {
            location = ES3.Location.File,
            encryptionType = ES3.EncryptionType.None,
            compressionType = ES3.CompressionType.None,
            format = ES3.Format.JSON,
            path = FILE_PATH,
            prettyPrint = true,
        };

        public void Save(SettingsData data)
        {
            ES3.Save(KEY, data, _es3Settings);
        }

        public SettingsData Load()
        {
            if (!ES3.FileExists(_es3Settings)) return new SettingsData();

            try
            {
                return ES3.Load<SettingsData>(KEY, _es3Settings);
            }
            catch
            {
                Logger.Error($"Failed to load {FILE_PATH}.");
                return new SettingsData();
            }
        }
    }
}