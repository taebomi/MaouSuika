using System;

namespace MaouSuika.Core
{
    [ES3Serializable, Serializable]
    public class AudioSettings
    {
        public VolumeSettings volume;

        public static AudioSettings CreateDefault()
        {
            return new AudioSettings
            {
                volume = VolumeSettings.CreateDefault(),
            };
        }

        public void Normalize()
        {
            volume ??= VolumeSettings.CreateDefault();
            volume.Normalize();
        }

        public AudioSettings Copy()
        {
            return new AudioSettings
            {
                volume = volume.Copy(),
            };
        }
    }
}
