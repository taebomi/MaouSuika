using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SOSG.System.Scene
{
    public static class SceneLoadHelper
    {
        public static event Func<SceneName, TransitionType, TransitionType, UniTaskVoid> SceneLoadRequested;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void InitializeSubsystemRegistration()
        {
            SceneLoadRequested = null;
        }

        public static void LoadScene(SceneName nextSceneName,
            TransitionType startTransitionType = TransitionType.TopAndBottom,
            TransitionType endTransitionType = TransitionType.TopAndBottom)
        {
            SceneLoadRequested?.Invoke(nextSceneName, startTransitionType, endTransitionType);
        }
    }
}