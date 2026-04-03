using System;
using System.Collections.Generic;
using TaeBoMi;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SOSG.System
{
    public static class TBMTimeScale
    {
        private static readonly LinkedList<MonoBehaviour> CallerList = new();
        private static readonly Dictionary<MonoBehaviour, float> TimeScaleDict = new();
        private static readonly HashSet<MonoBehaviour> PauseHashSet = new();

        public static event Action OnPaused;
        public static event Action OnUnPaused;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void InitializeOnSubsystemRegistration()
        {
            OnPaused = null;
            OnUnPaused = null;
            CallerList.Clear();
            TimeScaleDict.Clear();
            PauseHashSet.Clear();
        }

        // 씬 전환 시 모든 정보 초기화
        public static void Reset()
        {
            TimeScaleDict.Clear();
            CallerList.Clear();
            PauseHashSet.Clear();
            Time.timeScale = 1f;
        }

        public static void Pause()
        {
            Time.timeScale = 0f;
            OnPaused?.Invoke();
        }

        public static void UnPause()
        {
            Time.timeScale = 1f;
            OnUnPaused?.Invoke();
        }

        public static void Pause(MonoBehaviour caller)
        {
            TBMUtility.Log("# TimeScale - App Pause Requested");
            if (!PauseHashSet.Add(caller))
            {
                return;
            }

            Time.timeScale = 0f;
            OnPaused?.Invoke();
        }

        public static void UnPause(MonoBehaviour caller)
        {
            TBMUtility.Log("# TimeScale - App UnPause Requested");

            if (!PauseHashSet.Remove(caller))
            {
                return;
            }

            if (PauseHashSet.Count is 0)
            {
                OnUnPaused?.Invoke();
            }

            if (CallerList.Count is 0)
            {
                Time.timeScale = 1f;
            }
            else
            {
                Time.timeScale = TimeScaleDict[CallerList.First.Value];
            }
        }

        public static void Set(MonoBehaviour subject, float timeScale)
        {
            if (!TimeScaleDict.TryAdd(subject, timeScale))
            {
                TimeScaleDict[subject] = timeScale;
                if (CallerList.First.Value != subject)
                {
                    CallerList.Remove(subject);
                    CallerList.AddFirst(subject);
                }
            }
            else
            {
                CallerList.AddFirst(subject);
            }

            Time.timeScale = timeScale;
        }

        public static void Unset(MonoBehaviour subject)
        {
            CallerList.Remove(subject);
            TimeScaleDict.Remove(subject);
            Time.timeScale = CallerList.Count == 0
                ? 1f
                : TimeScaleDict[CallerList.First.Value];
        }
    }
}