using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TBM.Extensions;
using TBM.MaouSuika.Core.Scene;
using UnityEngine;
using UnityEngine.SceneManagement;
using SceneManager = UnityEngine.SceneManagement.SceneManager;

namespace TBM.MaouSuika.Core.Bootstrap
{
    public static class Bootstrapper
    {
        public static bool IsCompleted => _bootstrapTcs?.Task.Status == UniTaskStatus.Succeeded;

        private static UniTaskCompletionSource _bootstrapTcs;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void OnSubsystemRegistration()
        {
            _bootstrapTcs = null;
        }

        public static UniTask WaitForBootstrapAsync(CancellationToken token)
        {
            if (_bootstrapTcs != null) return _bootstrapTcs.Task.AttachExternalCancellation(token);

            _bootstrapTcs = new UniTaskCompletionSource();
            BootstrapAsync().Forget();
            return _bootstrapTcs.Task.AttachExternalCancellation(token);
        }

        private static async UniTaskVoid BootstrapAsync()
        {
            var token = Application.exitCancellationToken;
            try
            {
                await ProcessBootstrapSequence(token);
                _bootstrapTcs.TrySetResult();
            }
            catch (Exception ex)
            {
                Logger.Error("Bootstrap failed : " + ex.Message);
                _bootstrapTcs.TrySetException(ex);
            }
        }

        private static async UniTask ProcessBootstrapSequence(CancellationToken token)
        {
            var bootstrapScene = SceneManager.GetSceneByName(SceneNames.CORE);
            if (!bootstrapScene.IsValid() || !bootstrapScene.isLoaded)
            {
                await SceneManager.LoadSceneAsync(SceneNames.CORE, LoadSceneMode.Additive)
                    .ToUniTask(cancellationToken: token);
                bootstrapScene = SceneManager.GetSceneByName(SceneNames.CORE);
            }

            var gameManager = bootstrapScene.GetComponentInRoot<GameManager>();
            if (gameManager == null)
                throw new Exception($"{nameof(GameManager)} is not found in scene[{SceneNames.CORE}].");
            
            await gameManager.InitializeAsync(token);
        }
    }
}