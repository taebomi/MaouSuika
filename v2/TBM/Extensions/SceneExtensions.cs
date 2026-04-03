using UnityEngine.SceneManagement;

namespace TBM.Extensions
{
    public static class SceneExtensions
    {
        public static T GetComponentInRoot<T>(this Scene scene)
        {
            if (!scene.IsValid() || !scene.isLoaded) return default;

            var rootObjects = scene.GetRootGameObjects();
            foreach (var rootObject in rootObjects)
            {
                if (rootObject.TryGetComponent(out T component)) return component;
            }

            return default;
        }

        public static T GetComponentInSceneRoot<T>(string sceneName)
        {
            var scene = SceneManager.GetSceneByName(sceneName);
            return scene.GetComponentInRoot<T>();
        }
    }
}