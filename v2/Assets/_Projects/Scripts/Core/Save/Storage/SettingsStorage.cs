using System;
using UnityEngine;

namespace MaouSuika.Core
{
    public class SettingsStorage
    {
        private const string FILE_PATH = "Settings.sav";
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

        public SettingsData LoadOrDefault()
        {
            try
            {
                var defaults = new SettingsData();
                return ES3.Load(KEY, defaults, _es3Settings) ?? defaults;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to load settings from {_es3Settings.FullPath}.\n{ex}");
                return new SettingsData();
            }
        }
    }
}