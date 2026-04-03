using FMODUnity;
using UnityEngine;

namespace SOSG.System.Audio
{
    public class OverlordSfxController
    {
        private bool _isMute = false;
        private float _volume = 1f;

        public void SetMute(bool isMute)
        {
            _isMute = isMute;
        }

        public void SetVolume(float volume)
        {
            _volume = volume;
        }

        public void Play(EventReference sfxRef)
        {
            if (_isMute)
            {
                return;
            }

            var instance = RuntimeManager.CreateInstance(sfxRef);
            instance.setVolume(_volume);
            instance.start();
            instance.release();
        }
    }
}