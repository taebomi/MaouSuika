using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MaouSuika.Core
{
    public class SceneFlowManager : MonoBehaviour
    {
        public static SceneFlowManager Instance { get; private set; }

        private Scene _currentPrimaryScene;
        private bool _isChanging;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            Instance = null;
        }

        public void Initialize()
        {
            Instance = this;
            _currentPrimaryScene = SceneManager.GetActiveScene();
        }

        private void OnDestroy()
        {
            if (!ReferenceEquals(Instance, this)) return;

            Instance = null;
        }

        public async UniTask ChangePrimarySceneAsync(string sceneName)
        {
            if (_isChanging)
            {
                Debug.LogWarning($"이미 씬 전환 중...");
                return;
            }

            _isChanging = true;

            // todo 입력 차단
            // todo 로딩 화면 표시

            var token = destroyCancellationToken;
            var coreScene = SceneManager.GetSceneByName(SceneNames.CORE);
            SceneManager.SetActiveScene(coreScene);
            
            await SceneManager.UnloadSceneAsync(_currentPrimaryScene).ToUniTask(cancellationToken: token);

            await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive).ToUniTask(cancellationToken: token);
            var nextScene = SceneManager.GetSceneByName(sceneName);

            SceneManager.SetActiveScene(nextScene);
            _currentPrimaryScene = nextScene;

            // todo 로딩 화면 종료
            // todo 입력 복구

            _isChanging = false;
        }
    }
}