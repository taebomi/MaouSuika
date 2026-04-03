using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using SOSG.System.Scene;
using TaeBoMi;
using UnityEngine;
using UnityEngine.Serialization;

namespace SOSG.System.Scene
{
    public class LoadingScene : MonoBehaviour
    {
        public static LoadingScene Instance { get; private set; }

        [SerializeField] private LoadingMonster loadingMonster;
        [SerializeField] private LoadingFader fader;


        private void Awake()
        {
            Instance = this;

            SceneSetUpHelper.AddTask(SetUpAsync);
        }

        private async UniTask SetUpAsync()
        {
            await loadingMonster.SetUpAsync();
        }

        private void OnDestroy()
        {
            Instance = null;
            if (loadingMonster)
            {
                loadingMonster.TearDown();
            }
        }

        public void UpdateProgress(float progress)
        {
            loadingMonster.UpdateProgress(progress);
        }

        #region Fade

        public async UniTask ShowAsync(bool willHideConversation)
        {
            fader.SetSortingOrder(willHideConversation ? 32767 : 900);
            await fader.ShowAsync(destroyCancellationToken);
        }

        public async UniTask HideAsync(bool willHideConversation)
        {
            fader.SetSortingOrder(willHideConversation ? 32767 : 900);
            await fader.HideAsync(destroyCancellationToken);
        }

        #endregion
    }
}