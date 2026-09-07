using FMOD;
using FMOD.Studio;
using FMODUnity;
using Debug = UnityEngine.Debug;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace MaouSuika.Core
{
    public class BgmPlayer
    {
        private EventInstance _curBgm;
        private GUID _curBgmGuid;

        public void Play(EventReference bgmRef)
        {
            if (bgmRef.IsNull)
            {
                Debug.LogWarning("EventReference is null.");
                return;
            }

            if (_curBgm.isValid() && _curBgmGuid == bgmRef.Guid) return;

            Stop();

            _curBgmGuid = bgmRef.Guid;
            _curBgm = RuntimeManager.CreateInstance(bgmRef);
            _curBgm.start();
        }

        public void Stop(STOP_MODE stopMode = STOP_MODE.ALLOWFADEOUT)
        {
            if (_curBgm.isValid())
            {
                _curBgm.setCallback(null);
                _curBgm.stop(stopMode);
                _curBgm.release();
            }

            _curBgm.clearHandle();
            _curBgmGuid = default;
        }
    }
}