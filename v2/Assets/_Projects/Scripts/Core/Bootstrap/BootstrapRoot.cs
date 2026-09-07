using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MaouSuika.Core
{
    public class BootstrapRoot : MonoBehaviour
    {
        [SerializeField] private string defaultPrimarySceneName = SceneNames.GAMEPLAY;

        private async UniTaskVoid Start()
        {
            var token = destroyCancellationToken;

            // SceneFlowManager가 없으므로 Core 최초 로드를 직접 수행
            await EnsureCoreLoadedAsync(token);
            if (!GameManager.IsRegistered)
                throw new InvalidOperationException($"{nameof(GameManager)} was not found in Core Scene.");

            await GameManager.WaitUntilInitializedAsync(token);

            var targetPrimarySceneName = ResolveInitialPrimarySceneName();
            if (string.IsNullOrWhiteSpace(targetPrimarySceneName))
                throw new InvalidOperationException($"{nameof(targetPrimarySceneName)} is not configured.");

            // SceneFlowManager가 Scene 관리
            SceneFlowManager.Instance.ChangePrimarySceneAsync(targetPrimarySceneName).Forget();
        }

        private async UniTask EnsureCoreLoadedAsync(CancellationToken token)
        {
            var coreScene = SceneManager.GetSceneByName(SceneNames.CORE);
            if (coreScene.isLoaded) return;

            await SceneManager.LoadSceneAsync(SceneNames.CORE, LoadSceneMode.Additive)
                .ToUniTask(cancellationToken: token);
        }


        private string ResolveInitialPrimarySceneName()
        {
#if UNITY_EDITOR
            var editorTargetScene = PlayFromBootstrapSession.ConsumeTargetScene();
            if (!string.IsNullOrEmpty(editorTargetScene)) return editorTargetScene;
#endif
            return defaultPrimarySceneName;
        }
    }
}