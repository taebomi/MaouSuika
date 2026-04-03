using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Cysharp.Threading.Tasks;
using SOSG.System.Dialogue;
using SOSG.System.Input;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SOSG.System.Scene
{
    public class SceneLoadController : MonoBehaviour
    {
        [SerializeField] private SerializedDictionary<TransitionType, TransitionerBase> transitionerDict;

        private AsyncOperation _nextSceneLoadingOperation;

        private void OnEnable()
        {
            SceneLoadHelper.SceneLoadRequested += LoadSceneAsync;
        }

        private void OnDisable()
        {
            SceneLoadHelper.SceneLoadRequested -= LoadSceneAsync;
        }

        private async UniTaskVoid LoadSceneAsync(SceneName nextSceneName, TransitionType startTransitionType,
            TransitionType endTransitionType)
        {
            var startTransitioner = transitionerDict[startTransitionType];
            var endTransitioner = transitionerDict[endTransitionType];

            InputHelper.BlockInput(this);
            await startTransitioner.ShowAsync();
            await LoadLoadingSceneAsync();

            // Loading Scene
            await WaitForSceneSetUpFinished();
            InputHelper.UnblockInput(this);
            startTransitioner.SetActive(false);
            if (startTransitioner.WillHideConversation && endTransitioner.WillHideConversation is false)
            {
                DialogueHelper.ShowConversation();
            }

            await LoadingScene.Instance.ShowAsync(startTransitioner.WillHideConversation);
            _nextSceneLoadingOperation = await LoadNextSceneAsync(nextSceneName);
            LoadingScene.Instance.UpdateProgress(1f);
            await LoadingScene.Instance.HideAsync(startTransitioner.WillHideConversation);

            if (startTransitioner.WillHideConversation is false && endTransitioner.WillHideConversation)
            {
                DialogueHelper.HideConversation();
            }

            // Next Scene
            InputHelper.BlockInput(this);
            endTransitioner.SetActive(true);
            await WaitForNextSceneLoadedAsync();
            await WaitForSceneSetUpFinished();
            await endTransitioner.HideAsync();
            InputHelper.UnblockInput(this);

            _nextSceneLoadingOperation = null;
        }

        private UniTask LoadLoadingSceneAsync()
        {
            var tcs = new UniTaskCompletionSource();

            var operation = SceneManager.LoadSceneAsync(SceneName.Loading.ToString());
            if (operation is null)
            {
                throw new Exception("# SceneLoader - LoadLoadingSceneAsync - Loading Scene이 일치하지 않음.");
            }

            operation.completed += _ => { tcs.TrySetResult(); };
            return tcs.Task;
        }

        private UniTask WaitForSceneSetUpFinished()
        {
            if (SceneSetUpHelper.IsCompleted)
            {
                return UniTask.CompletedTask;
            }

            var tcs = new UniTaskCompletionSource();
            SceneSetUpHelper.Completed += OnSceneSetUpFinished;
            return tcs.Task;

            void OnSceneSetUpFinished()
            {
                SceneSetUpHelper.Completed -= OnSceneSetUpFinished;
                tcs.TrySetResult();
            }
        }

        private async UniTask<AsyncOperation> LoadNextSceneAsync(SceneName sceneName)
        {
            var operation = SceneManager.LoadSceneAsync(sceneName.ToString());
            if (operation is null)
            {
                throw new Exception("# SceneLoader - LoadNextSceneAsync - Scene이 일치하지 않음.");
            }

            operation.allowSceneActivation = false;

            while (operation.progress < 0.9f)
            {
                LoadingScene.Instance.UpdateProgress(operation.progress);
                await UniTask.Yield();
            }

            return operation;
        }

        private UniTask WaitForNextSceneLoadedAsync()
        {
            var tcs = new UniTaskCompletionSource();
            _nextSceneLoadingOperation.completed += _ => { tcs.TrySetResult(); };

            _nextSceneLoadingOperation.allowSceneActivation = true;
            return tcs.Task;
        }
    }
}