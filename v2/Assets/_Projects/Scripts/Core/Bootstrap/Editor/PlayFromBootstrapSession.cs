#if UNITY_EDITOR

using UnityEditor;

namespace MaouSuika.Core
{
    public static class PlayFromBootstrapSession
    {
        private const string TARGET_SCENE_KEY = "MaouSuika.PlayFromBootstrap.TargetScene";

        public static void SetTargetScene(string sceneName)
        {
            SessionState.SetString(TARGET_SCENE_KEY, sceneName);
        }

        public static void ClearTargetScene()
        {
            SessionState.EraseString(TARGET_SCENE_KEY);
        }

        public static string ConsumeTargetScene()
        {
            var scenePath = SessionState.GetString(TARGET_SCENE_KEY, string.Empty);
            SessionState.EraseString(TARGET_SCENE_KEY);
            return scenePath;
        }
    }
}

#endif