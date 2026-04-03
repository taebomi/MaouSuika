using System;
using Cysharp.Threading.Tasks;
using FMODUnity;
using TBM.MaouSuika.Core.Save;
using TBM.MaouSuika.Core.Settings;
using UnityEngine;
using AudioSettings = TBM.MaouSuika.Core.Save.AudioSettings;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace TBM.MaouSuika.Core.Audio
{
    [Serializable]
    public class AudioManager : CoreManager<AudioManager>, ISettingsDataHandler
    {
        [SerializeField] private BgmPlayer bgmPlayer;
        [SerializeField] private SfxPlayer sfxPlayer;
        [SerializeField] private SnapshotHandler snapshotHandler;
        [SerializeField] private VolumeController volumeController;
        
        public async UniTask InitializeAsync(AudioSettings settings)
        {
            await WaitForFmodReadyAsync();
            volumeController.Initialize(settings.volume);
        }
        
        public void PlayBgm(EventReference bgmRef)
        {
            bgmPlayer.Play(bgmRef);
        }

        public void StopBgm(STOP_MODE stopMode = STOP_MODE.ALLOWFADEOUT)
        {
            bgmPlayer.Stop(stopMode);
        }

        public void PlaySfx(EventReference sfxRef)
        {
            sfxPlayer.Play(sfxRef);
        }

        public void PlaySfx(EventReference sfxRef, float pitch)
        {
            sfxPlayer.Play(sfxRef, pitch);
        }

        public void SetSnapshotActive(EventReference snapshotRef, bool value)
        {
            snapshotHandler.SetActive(snapshotRef, value);
        }

        public void SetSnapshotParameter(EventReference snapshotRef, string paramName, float value)
        {
            snapshotHandler.SetParameter(snapshotRef, paramName, value);
        }

        #region Config

        public void OnSaveData(SettingsData data)
        {
            data.audio.volume = volumeController.GetSettings();
        }

        public void OnLoadData(SettingsData data)
        {
            var audioSettings = data.audio;
            volumeController.ApplySettings(audioSettings.volume);
        }

        public void SetMasterMuted(bool muted)
        {
            volumeController.SetMasterMuted(muted);
        }

        public void SetMasterVolume(float volume)
        {
            volumeController.SetMasterVolume(volume);
        }

        public void SetBgmMuted(bool muted)
        {
            volumeController.SetBgmMuted(muted);
        }

        public void SetBgmVolume(float volume)
        {
            volumeController.SetBgmVolume(volume);
        }

        public void SetSfxMuted(bool muted)
        {
            volumeController.SetSfxMuted(muted);
        }

        public void SetSfxVolume(float volume)
        {
            volumeController.SetSfxVolume(volume);
        }

        #endregion

        private async UniTask WaitForFmodReadyAsync()
        {
            RuntimeManager.WaitForAllSampleLoading();
            await UniTask.WaitUntil(() => RuntimeManager.IsInitialized && !RuntimeManager.AnySampleDataLoading());
        }
    }
}