using UnityEngine;
using UnityEngine.SceneManagement;
using Enum = System.Enum;

namespace SOSG.System.Scene
{
    public static class SceneContext
    {
        public static SceneName CurSceneName { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void InitializeSubsystemRegistration()
        {
            var sceneName = SceneManager.GetActiveScene().name;
            CurSceneName = Enum.Parse<SceneName>(sceneName);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeBeforeSceneLoaded()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private static void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode loadSceneMode)
        {
            CurSceneName = Enum.Parse<SceneName>(scene.name);
        }
    }
}