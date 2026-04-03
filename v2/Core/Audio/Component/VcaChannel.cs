using System;
using FMOD.Studio;
using FMODUnity;
using TBM.MaouSuika.Core.Save;
using UnityEngine;

namespace TBM.MaouSuika.Core.Audio
{
    public class VcaChannel
    {
        public float Volume { get; private set; }
        public bool IsMuted { get; private set; }

        private readonly VCA _vca;

        public VcaChannel(string vcaPath, VcaChannelSettings settings)
        {
            _vca = RuntimeManager.GetVCA(vcaPath);

            Volume = Mathf.Clamp01(settings.volume);
            IsMuted = settings.muted;

            _vca.setVolume(IsMuted ? 0f : Volume);
        }

        public void SetVolume(float volume)
        {
            volume = Mathf.Clamp01(volume);
            if (Mathf.Approximately(Volume, volume)) return;

            Volume = volume;
            if (IsMuted) return;

            _vca.setVolume(Volume);
        }

        public void SetMuted(bool muted)
        {
            if (IsMuted == muted) return;
            
            IsMuted = muted;
            _vca.setVolume(IsMuted ? 0f : Volume);
        }

        public void ApplySettings(VcaChannelSettings settings)
        {
            SetVolume(settings.volume);
            SetMuted(settings.muted);
        }

        public VcaChannelSettings GetSettings()
        {
            return new VcaChannelSettings(IsMuted, Volume);
        }
    }
}