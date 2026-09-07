using FMODUnity;

namespace MaouSuika.Core
{
    public class SfxPlayer
    {
        public void PlayOneShot(EventReference sfxRef, string parameterName, float value)
        {
            if (sfxRef.IsNull) return;

            var instance = RuntimeManager.CreateInstance(sfxRef);
            instance.setParameterByName(parameterName, value);
            instance.start();
            instance.release();
        }

        public void PlayOneShot(EventReference sfxRef)
        {
            if (sfxRef.IsNull) return;

            var instance = RuntimeManager.CreateInstance(sfxRef);
            instance.start();
            instance.release();
        }

        public void PlayOneShot(EventReference sfxRef, float pitch)
        {
            if (sfxRef.IsNull) return;

            var instance = RuntimeManager.CreateInstance(sfxRef);
            instance.setPitch(pitch);
            instance.start();
            instance.release();
        }
    }
}