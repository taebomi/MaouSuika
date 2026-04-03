using System;
using UnityEngine;

namespace SOSG.System.Input
{
    public static class InputHelper
    {
        public static event Action<int> InputBlockRequested;
        public static event Action<int> InputUnblockRequested;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void InitializeOnSubsystemRegistration()
        {
            InputBlockRequested = null;
            InputUnblockRequested = null;
        }

        public static void BlockInput(MonoBehaviour subject)
        {
            var id = subject.GetInstanceID();
            InputBlockRequested?.Invoke(id);
        }

        public static void UnblockInput(MonoBehaviour subject)
        {
            var id = subject.GetInstanceID();
            InputUnblockRequested?.Invoke(id);
        }
    }
}