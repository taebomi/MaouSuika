using System;
using FMODUnity;
using UnityEngine;

namespace SOSG.System.Audio
{
    public static class AudioSystemHelper
    {
        // bgm
        public static event Action<EventReference> BgmPlayRequested;
        public static event Action<EventReference, Action<string>> BgmWithMarkerPlayRequested;
        public static event Action<float> BgmPitchChangeRequested;
        public static event Action<int> BgmTimelinePositionChangeRequested;

        // sfx
        public static event Action<EventReference> SfxPlayRequested;
        public static event Action<EventReference, float> SfxWithPitchPlayRequested;


        // overlord sfx
        public static event Action<EventReference> OverlordSfxPlayRequested;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void InitializeOnSubsystemRegistration()
        {
            BgmPlayRequested = null;
            BgmWithMarkerPlayRequested = null;
            BgmTimelinePositionChangeRequested = null;
            BgmPitchChangeRequested = null;

            SfxPlayRequested = null;
            SfxWithPitchPlayRequested = null;

            OverlordSfxPlayRequested = null;
        }

        public static void PlayBgm(EventReference bgmRef)
        {
            BgmPlayRequested?.Invoke(bgmRef);
        }

        public static void PlayBgm(EventReference bgmRef, Action<string> onMarkerChanged)
        {
            BgmWithMarkerPlayRequested?.Invoke(bgmRef, onMarkerChanged);
        }

        public static void ChangeBgmTimelinePosition(int position)
        {
            BgmTimelinePositionChangeRequested?.Invoke(position);
        }
        
        public static void SetBgmPitch(float pitch)
        {
            BgmPitchChangeRequested?.Invoke(pitch);
        }

        public static void PlaySfx(EventReference sfxRef)
        {
            SfxPlayRequested?.Invoke(sfxRef);
        }

        public static void PlaySfx(EventReference sfxRef, float pitch)
        {
            SfxWithPitchPlayRequested?.Invoke(sfxRef, pitch);
        }

        public static void PlayOverlordSfx(EventReference sfxRef)
        {
            OverlordSfxPlayRequested?.Invoke(sfxRef);
        }

        public static void PlayAudio(EventReference audioRef, float volume)
        {
            var instance = RuntimeManager.CreateInstance(audioRef);
            instance.setVolume(volume);
            instance.start();
            instance.release();
        }
    }
}