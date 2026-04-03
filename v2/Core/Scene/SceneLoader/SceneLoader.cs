using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TBM.Extensions;
using UnityEngine;

namespace TBM.MaouSuika.Core.Scene
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] private TransitionController transitionController;
        [SerializeField] private LoadingScreenController loadingScreenController;

        public event Action SceneLoadStarted;
        public event Action SceneLoadFailed; // todo + 실패 이유 추가해주기
        public event Action SceneLoadCompleted;

        public async UniTask LoadSequenceAsync(string sceneName,
            TransitionType transitionIn, TransitionType transitionOut,
            SceneLoadPayload payload)
        {
            SceneLoadStarted?.Invoke();

            // Transition In
            await transitionController.CoverAsync(transitionIn, destroyCancellationToken);
            loadingScreenController.Show(payload?.LoadingData);
            await transitionController.RevealAsync(transitionIn, destroyCancellationToken);

            // Load Next Scene
            var success = await LoadSceneAsync(sceneName, destroyCancellationToken);
            if (!success)
            {
                SceneLoadFailed?.Invoke();
                return;
            }

            // # Initialize Scene
            var sceneController = SceneExtensions.GetComponentInSceneRoot<SceneControllerBase>(sceneName);
            if (sceneController == null)
            {
                SceneLoadFailed?.Invoke();
                return;
            }

            await sceneController.InitializeSceneAsync(payload?.NextSceneData);
            loadingScreenController.UpdateProgress(1f);
            
            // # Transition Out
            await transitionController.CoverAsync(transitionOut, destroyCancellationToken);
            loadingScreenController.Hide();
            await transitionController.RevealAsync(transitionOut, destroyCancellationToken);

            // # Start New Scene
            sceneController.ProcessScene();

            SceneLoadCompleted?.Invoke();
        }

        private async UniTask<bool> LoadSceneAsync(string sceneName, CancellationToken token)
        {
            // Clean up
            await Resources.UnloadUnusedAssets();
            GC.Collect();

            // Load Scene
            var loadOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
            if (loadOperation == null)
            {
                Logger.Error($"Scene {sceneName} is not exists.");
                return false;
            }

            loadOperation.allowSceneActivation = false;
            while (!loadOperation.isDone)
            {
                var progress = Mathf.Clamp01(loadOperation.progress / 1f);
                loadingScreenController.UpdateProgress(progress);

                if (loadOperation.progress >= 0.9f) break;
                await UniTask.Yield(token);
            }

            loadOperation.allowSceneActivation = true;
            await UniTask.WaitUntil(() => loadOperation.isDone, cancellationToken: token);
            return true;
        }
    }
}