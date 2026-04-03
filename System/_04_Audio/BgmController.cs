using System;
using System.Runtime.InteropServices;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace SOSG.System.Audio
{
    public class BgmController
    {
        private EventInstance _curBgmInstance;
        private EventInstance _lastBgmInstance;

        private TimelineInfo _timelineInfo;
        private GCHandle _timelineHandle;
        private EVENT_CALLBACK _timelineEventCallback;

        private float _volume = 1f;
        private float _pitch = 1f;

        public void SetVolume(float volume)
        {
            _volume = volume;
            if (_curBgmInstance.isValid())
            {
                _curBgmInstance.setVolume(_volume);
            }
        }

        public void SetPitch(float pitch)
        {
            _pitch = pitch;
            if (_curBgmInstance.isValid())
            {
                _curBgmInstance.setPitch(_pitch);
            }
        }


        public void Play(EventReference bgmRef)
        {
            if (_curBgmInstance.isValid())
            {
                _lastBgmInstance = _curBgmInstance;
                _lastBgmInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                _lastBgmInstance.release();
            }

            _curBgmInstance = RuntimeManager.CreateInstance(bgmRef);
            _curBgmInstance.setVolume(_volume);
            _curBgmInstance.setPitch(_pitch);
            _curBgmInstance.start();
        }

        public void Play(EventReference bgmRef, Action<string> markerChanged)
        {
            if (_curBgmInstance.isValid())
            {
                _lastBgmInstance = _curBgmInstance;
                _lastBgmInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                _lastBgmInstance.release();
            }

            _timelineInfo = new TimelineInfo()
            {
                MarkerChanged = markerChanged
            };
            _timelineHandle = GCHandle.Alloc(_timelineInfo);
            _timelineEventCallback = TimelineEventCallback;

            _curBgmInstance = RuntimeManager.CreateInstance(bgmRef);
            _curBgmInstance.setUserData(GCHandle.ToIntPtr(_timelineHandle));
            _curBgmInstance.setCallback(_timelineEventCallback, EVENT_CALLBACK_TYPE.TIMELINE_MARKER);
            _curBgmInstance.setVolume(_volume);
            _curBgmInstance.setPitch(_pitch);
            _curBgmInstance.start();
        }

        public void SetTime(int time)
        {
            _curBgmInstance.setTimelinePosition(time);
        }

        public void Stop()
        {
            if (_curBgmInstance.isValid())
            {
                _curBgmInstance.setCallback(null);
                _curBgmInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                _curBgmInstance.release();
            }
        }

        [AOT.MonoPInvokeCallback(typeof(EVENT_CALLBACK))]
        private static RESULT TimelineEventCallback(EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr)
        {
            var instance = new EventInstance(instancePtr);

            var result = instance.getUserData(out var timelineInfoPtr);
            if (result != RESULT.OK)
            {
                Debug.LogError($"Timeline Callback Error: {result}");
            }
            else if (timelineInfoPtr != IntPtr.Zero)
            {
                var timelineHandle = GCHandle.FromIntPtr(timelineInfoPtr);
                var timelineInfo = (TimelineInfo)timelineHandle.Target;
                switch (type)
                {
                    case EVENT_CALLBACK_TYPE.TIMELINE_MARKER:
                        var parameter =
                            (TIMELINE_MARKER_PROPERTIES)Marshal.PtrToStructure(parameterPtr,
                                typeof(TIMELINE_MARKER_PROPERTIES));
                        // timelineInfo.LastMarker = parameter.name;
                        timelineInfo.MarkerChanged?.Invoke(parameter.name);
                        break;
                    case EVENT_CALLBACK_TYPE.DESTROYED:
                        timelineHandle.Free();
                        break;
                }
            }

            return RESULT.OK;
        }

        private class TimelineInfo
        {
            // public StringWrapper LastMarker = new();
            public Action<string> MarkerChanged;
        }
    }
}