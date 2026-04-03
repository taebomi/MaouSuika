using System;
using SOSG.System.Dialogue;
using SOSG.System.Input;
using UnityEngine;

namespace SOSG.System
{
    public static class WaitIndicatorHelper
    {
        public static event Action<bool> WaitIndicatorActiveRequested;
    
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void InitializeOnSubsystemRegistration()
        {
            WaitIndicatorActiveRequested = null;
        }

        public static void Show(MonoBehaviour caller)
        {
            TBMTimeScale.Pause(caller);
            DialogueHelper.Pause();
            InputHelper.BlockInput(caller);
            WaitIndicatorActiveRequested?.Invoke(true);
        }

        public static void Hide(MonoBehaviour caller)
        {
            TBMTimeScale.UnPause(caller);
            DialogueHelper.UnPause();
            InputHelper.UnblockInput(caller);
            WaitIndicatorActiveRequested?.Invoke(false);
        }
    }
}