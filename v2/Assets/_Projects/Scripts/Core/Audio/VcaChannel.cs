using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace MaouSuika.Core
{
    public class VcaChannel
    {
        private VCA _vca;

        public float Volume { get; private set; }
        public bool IsMuted { get; private set; }

        public VcaChannel(string vcaPath)
        {
            _vca = RuntimeManager.GetVCA(vcaPath);
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

        public VcaChannelSettings CaptureSettings()
        {
            return new VcaChannelSettings(IsMuted, Volume);
        }
    }
}