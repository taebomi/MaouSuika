using System;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using Sirenix.OdinInspector;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace TBM.MaouSuika.Core.Audio
{
    public class BgmPlayer : MonoBehaviour
    {
        private EventInstance _curBgm;
        private GUID _curBgmGuid;

        private void OnDestroy()
        {
            Stop(STOP_MODE.IMMEDIATE);
        }

        public void Play(EventReference bgmRef)
        {
            if (bgmRef.IsNull)
            {
                Logger.Warning($"Event Reference is null.");
                return;
            }

            if (_curBgm.isValid() && _curBgmGuid == bgmRef.Guid)
            {
                return;
            }

            Stop();

            _curBgmGuid = bgmRef.Guid;

            _curBgm = RuntimeManager.CreateInstance(bgmRef);
            _curBgm.start();
        }

        public void Stop(STOP_MODE stopMode = STOP_MODE.ALLOWFADEOUT)
        {
            if (!_curBgm.isValid()) return;

            _curBgm.setCallback(null);
            _curBgm.stop(stopMode);
            _curBgm.release();
        }

#if UNITY_EDITOR

        [Title("Debug_EditorOnly")]
        [SerializeField] private EventReference debug_eventRef;

        [Button("Play")]
        public void Debug_Play()
        {
            Play(debug_eventRef);
        }

        [Button("Stop")]
        public void Debug_Stop()
        {
            Stop();
        }

#endif
    }
}