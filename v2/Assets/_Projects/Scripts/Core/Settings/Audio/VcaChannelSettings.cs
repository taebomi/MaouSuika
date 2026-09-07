using System;
using UnityEngine;

namespace MaouSuika.Core
{
    [Serializable]
    public class VcaChannelSettings
    {
        public bool muted;
        public float volume;

        public VcaChannelSettings(bool muted, float volume)
        {
            this.muted = muted;
            this.volume = volume;
        }

        public void CopyFrom(VcaChannelSettings source)
        {
            muted = source.muted;
            volume = source.volume;
        }

        public void Normalize(VcaChannelSettings defaults)
        {
            if (float.IsNaN(volume) || float.IsInfinity(volume))
                volume = defaults.volume;

            volume = Mathf.Clamp01(volume);
        }

        public VcaChannelSettings Copy()
        {
            return new VcaChannelSettings(muted, volume);
        }
    }
}