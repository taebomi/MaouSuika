using System;

namespace MaouSuika.Core
{
    [Serializable]
    public class VolumeSettings
    {
        public VcaChannelSettings master;
        public VcaChannelSettings bgm;
        public VcaChannelSettings sfx;

        public static VolumeSettings CreateDefault()
        {
            return new VolumeSettings
            {
                master = new VcaChannelSettings(false, 1f),
                bgm = new VcaChannelSettings(false, 1f),
                sfx = new VcaChannelSettings(false, 1f),
            };
        }

        public void Normalize()
        {
            var defaults = CreateDefault();

            master ??= defaults.master;
            bgm ??= defaults.bgm;
            sfx ??= defaults.sfx;

            master.Normalize(defaults.master);
            bgm.Normalize(defaults.bgm);
            sfx.Normalize(defaults.sfx);
        }

        public VolumeSettings Copy()
        {
            return new VolumeSettings
            {
                master = master.Copy(),
                bgm = bgm.Copy(),
                sfx = sfx.Copy(),
            };
        }
    }
}
