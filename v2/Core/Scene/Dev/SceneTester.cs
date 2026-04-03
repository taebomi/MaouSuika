using System;
using Cysharp.Threading.Tasks;
using TBM.MaouSuika.Core.Bootstrap;
using UnityEngine;

namespace TBM.MaouSuika.Core.Scene
{
    public class SceneTester : MonoBehaviour
    {
        [SerializeField] private SceneTestPayloadSO payload;

        private SceneControllerBase _sceneController;

        private async UniTaskVoid Start()
        {
            if (Bootstrapper.IsCompleted) return;

            var sceneControllers =
                FindObjectsByType<SceneControllerBase>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            if (sceneControllers.Length != 1)
            {
                Logger.Error($"Scene Controllers count is not 1. [{sceneControllers.Length}]");
                return;
            }

            await Bootstrapper.WaitForBootstrapAsync(destroyCancellationToken);
            var sceneController = sceneControllers[0];
            await sceneController.InitializeSceneAsync(payload.CreatePayload());
            sceneController.ProcessScene();
        }
    }
}