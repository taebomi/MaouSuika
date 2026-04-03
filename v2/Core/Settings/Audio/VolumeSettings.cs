using System;
using TBM.MaouSuika.Core.Audio;

namespace TBM.MaouSuika.Core.Settings
{
    [Serializable, ES3Serializable]
    public class VolumeSettings
    {
        public VcaChannelSettings master;
        public VcaChannelSettings bgm;
        public VcaChannelSettings sfx;

        public VolumeSettings()
        {
            master = VcaChannelSettings.Default;
            bgm = VcaChannelSettings.Default;
            sfx = VcaChannelSettings.Default;
        }

        public VolumeSettings(VcaChannelSettings master, VcaChannelSettings bgm, VcaChannelSettings sfx)
        {
            this.master = master;
            this.bgm = bgm;
            this.sfx = sfx;
        }
    }
}