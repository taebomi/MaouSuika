using System;

namespace TBM.MaouSuika.Core.Audio
{
    [Serializable]
    public struct VcaChannelSettings
    {
        public bool muted;
        public float volume;

        public VcaChannelSettings(bool muted, float volume)
        {
            this.muted = muted;
            this.volume = volume;
        }

        public static VcaChannelSettings Default => new(false, 1f);
    }
}