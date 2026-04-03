using System;
using System.Collections.Generic;
using FMOD;
using FMODUnity;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TBM.MaouSuika.Core.Audio
{
    // 씬 전환 시마다 sfxCooldown Dictionary를 Clear 해주는 식으로 최적화 가능함
    // 신경 안 써도 될 수준이므로 일단 생략
    public class SfxPlayer : MonoBehaviour
    {
        private readonly Dictionary<GUID, float> _sfxCooldowns = new();

        private const float MIN_SFX_INTERVAL = 0.05f;

        public void Play(EventReference sfxRef)
        {
            if (sfxRef.IsNull) return;
            if (ShouldSkip(sfxRef.Guid)) return;

            var instance = RuntimeManager.CreateInstance(sfxRef);
            instance.start();
            instance.release();
        }

        public void Play(EventReference sfxRef, float pitch)
        {
            if (sfxRef.IsNull) return;
            if (ShouldSkip(sfxRef.Guid)) return;

            var instance = RuntimeManager.CreateInstance(sfxRef);
            instance.setPitch(pitch);
            instance.start();
            instance.release();
        }

        private bool ShouldSkip(GUID guid)
        {
            var curTime = Time.unscaledTime;

            if (_sfxCooldowns.TryGetValue(guid, out var lastPlayTime) &&
                curTime - lastPlayTime < MIN_SFX_INTERVAL) return true;

            _sfxCooldowns[guid] = curTime;
            return false;
        }

#if UNITY_EDITOR

        [SerializeField] private EventReference debug_eventRef;
        [PropertyRange(0f, 1f)]
        [SerializeField] private float debug_pitch;

        [Button]
        private void Debug_Play()
        {
            Play(debug_eventRef, debug_pitch);
        }

#endif
    }
}