using System;
using UnityEngine.Serialization;

namespace SOSG.System.Setting
{
    [Serializable, ES3Serializable]
    public class AudioSetting
    {
        public bool bgmMute;
        public float bgmVolume;
        public bool sfxMute;
        public float sfxVolume;
        public bool overlordSfxMute;
        public float overlordSfxVolume;
    
        public AudioSetting()
        {
            bgmMute = false;
            bgmVolume = 1f;
            sfxMute = false;
            sfxVolume = 1f;
            overlordSfxMute = false;
            overlordSfxVolume = 1f;
        }
    
        public AudioSetting(AudioSetting audioSetting)
        {
            bgmMute = audioSetting.bgmMute;
            bgmVolume = audioSetting.bgmVolume;
            sfxMute = audioSetting.sfxMute;
            sfxVolume = audioSetting.sfxVolume;
            overlordSfxMute = audioSetting.overlordSfxMute;
            overlordSfxVolume = audioSetting.overlordSfxVolume;
        }

        public bool IsEqual(AudioSetting audioSetting)
        {
            // ReSharper disable once ReplaceWithSingleAssignment.True
            var isEqual = true;

            if (bgmMute != audioSetting.bgmMute)
            {
                isEqual = false;
            }

            if (Math.Abs(bgmVolume - audioSetting.bgmVolume) > 0.01f)
            {
                isEqual = false;
            }

            if (sfxMute != audioSetting.sfxMute)
            {
                isEqual = false;
            }

            if (Math.Abs(sfxVolume - audioSetting.sfxVolume) > 0.01f)
            {
                isEqual = false;
            }
        
            if( overlordSfxMute != audioSetting.overlordSfxMute)
            {
                isEqual = false;
            }
        
            if(Math.Abs(overlordSfxVolume - audioSetting.overlordSfxVolume) > 0.01f)
            {
                isEqual = false;
            }

            return isEqual;
        }

        public void SetBgmMute(bool value)
        {
            bgmMute = value;
        }

        public void SetSfxMute(bool value)
        {
            sfxMute = value;
        }

        public void SetBgmVolume(float value)
        {
            bgmVolume = value;
        }

        public void SetSfxVolume(float value)
        {
            sfxVolume = value;
        }
    
        public void SetOverlordSfxMute(bool value)
        {
            overlordSfxMute = value;
        }
    
        public void SetOverlordSfxVolume(float value)
        {
            overlordSfxVolume = value;
        }
    }
}