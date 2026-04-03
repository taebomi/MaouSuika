using System;
using UnityEngine;

namespace TBM.MaouSuika.Core.UI
{
    public static class UIService
    {
        public static IUIService Instance { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void OnSubsystemRegistration()
        {
            Instance = null;
        }

        public static void Register(IUIService uiService)
        {
            if (Instance != null)
            {
                throw new Exception("Already initialized.");
            }

            Instance = uiService;
        }

        public static void Unregister(IUIService uiService)
        {
            if (Instance == null)
            {
                throw new Exception("Not initialized.");
            }

            if (Instance != uiService)
            {
                throw new Exception("Instance not matched.");
            }

            Instance = null;
        }
    }
}