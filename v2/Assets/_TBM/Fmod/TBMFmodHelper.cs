using FMODUnity;

namespace TBM.Core
{
    public static class FmodAudio
    {
        public static void PlayOneShot(EventReference eventRef)
        {
            var instance = RuntimeManager.CreateInstance(eventRef);
            instance.start();
            instance.release();
        }

        public static void PlayOneShot(EventReference eventRef, string paramName, float value)
        {
            var instance = RuntimeManager.CreateInstance(eventRef);
            instance.setParameterByName(paramName, value);
            instance.start();
            instance.release();
        }
    }
}