#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace MaouSuika.Core
{
    [InitializeOnLoad]
    public static class PlayFromBootstrap
    {
        public const string BOOTSTRAP_PATH = "Assets/_Projects/Scenes/Bootstrap.unity";

        static PlayFromBootstrap()
        {
            var bootstrap = AssetDatabase.LoadAssetAtPath<SceneAsset>(BOOTSTRAP_PATH);
            EditorSceneManager.playModeStartScene = bootstrap;

            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state != PlayModeStateChange.ExitingEditMode) return;

            var targetSceneName = SceneManager.GetActiveScene().name;
            if (string.IsNullOrEmpty(targetSceneName) || targetSceneName is SceneNames.BOOTSTRAP or SceneNames.CORE)
            {
                PlayFromBootstrapSession.ClearTargetScene();
                return;
            }

            PlayFromBootstrapSession.SetTargetScene(targetSceneName);
        }
    }
}

#endif