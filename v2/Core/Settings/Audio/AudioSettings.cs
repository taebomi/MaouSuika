using System;
using TBM.MaouSuika.Core.Settings;

namespace TBM.MaouSuika.Core.Save
{
    [ES3Serializable, Serializable]
    public class AudioSettings
    {
        public VolumeSettings volume;

        public AudioSettings()
        {
            volume = new VolumeSettings();
        }

        public AudioSettings(VolumeSettings volume)
        {
            this.volume = volume;
        }
    }
}