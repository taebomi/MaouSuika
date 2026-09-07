using System;
using FMODUnity;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace MaouSuika.Core
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        private BgmPlayer _bgmPlayer;
        private VolumeController _volumeController;
        private SfxPlayer _sfxPlayer;
        private SnapshotHandler _snapshotHandler;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            Instance = null;
        }

        public void Initialize()
        {
            Instance = this;
            _bgmPlayer = new BgmPlayer();
            _volumeController = new VolumeController();
            _sfxPlayer = new SfxPlayer();
            _snapshotHandler = new SnapshotHandler();
        }

        private void OnDestroy()
        {
            if (ReferenceEquals(Instance, this) is false) return;

            _bgmPlayer?.Stop(STOP_MODE.IMMEDIATE);
            _snapshotHandler?.Dispose();
            Instance = null;
        }

        public void ApplySettings(AudioSettings settings)
        {
            _volumeController.ApplySettings(settings.volume);
        }

        public AudioSettings CaptureSettings()
        {
            return new AudioSettings
            {
                volume = _volumeController.CaptureSettings(),
            };
        }

        public void SetMasterVolume(float volume)
        {
            _volumeController.SetMasterVolume(volume);
        }

        public void SetMasterMuted(bool muted)
        {
            _volumeController.SetMasterMuted(muted);
        }

        public void SetBgmVolume(float volume)
        {
            _volumeController.SetBgmVolume(volume);
        }

        public void SetBgmMuted(bool muted)
        {
            _volumeController.SetBgmMuted(muted);
        }

        public void SetSfxVolume(float volume)
        {
            _volumeController.SetSfxVolume(volume);
        }

        public void SetSfxMuted(bool muted)
        {
            _volumeController.SetSfxMuted(muted);
        }

        public void PlayBgm(EventReference bgmRef)
        {
            _bgmPlayer.Play(bgmRef);
        }

        public void StopBgm(STOP_MODE stopMode = STOP_MODE.ALLOWFADEOUT)
        {
            _bgmPlayer.Stop(stopMode);
        }

        public void PlaySfxOneShot(EventReference sfxRef)
        {
            _sfxPlayer.PlayOneShot(sfxRef);
        }

        public void PlaySfxOneShot(EventReference sfxRef, float pitch)
        {
            _sfxPlayer.PlayOneShot(sfxRef, pitch);
        }

        public void PlaySfxOneShot(EventReference sfxRef, string paramName, float volume)
        {
            _sfxPlayer.PlayOneShot(sfxRef, paramName, volume);
        }

        public void StartSnapshot(EventReference snapshotRef)
        {
            _snapshotHandler.Start(snapshotRef);
        }

        public void StopSnapshot(EventReference snapshotRef)
        {
            _snapshotHandler.Stop(snapshotRef);
        }

        public void SetSnapshot(EventReference snapshotRef, string paramName, float value)
        {
            _snapshotHandler.Set(snapshotRef, paramName, value);
        }
    }
}