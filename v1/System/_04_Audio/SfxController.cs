using FMODUnity;

namespace SOSG.System.Audio
{
    public class SfxController
    {
        private float _volume = 1f;

        public void SetVolume(float volume)
        {
            _volume = volume;
        }


        public void Play(EventReference sfxRef)
        {
            var instance = RuntimeManager.CreateInstance(sfxRef);
            instance.setVolume(_volume);
            instance.start();
            instance.release();
        }

        public void Play(EventReference sfxRef, float pitch)
        {
            var instance = RuntimeManager.CreateInstance(sfxRef);
            instance.setVolume(_volume);
            instance.setPitch(pitch);
            instance.start();
            instance.release();
        }
    }
}