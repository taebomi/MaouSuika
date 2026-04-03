using Cysharp.Threading.Tasks;
using TBM.MaouSuika.Core.Input;
using UnityEngine;

namespace TBM.MaouSuika.Core.Scene
{
    public class SceneManager : CoreManager<SceneManager>
    {
        [SerializeField] private SceneTableSO sceneTable;

        [SerializeField] private SceneLoader loader;

        private void OnEnable()
        {
            loader.SceneLoadStarted += HandleLoadStarted;
            loader.SceneLoadFailed += HandleLoadFailed;
            loader.SceneLoadCompleted += HandleLoadCompleted;
        }

        private void OnDisable()
        {
            if (loader != null)
            {
                loader.SceneLoadStarted -= HandleLoadStarted;
                loader.SceneLoadFailed -= HandleLoadFailed;
                loader.SceneLoadCompleted -= HandleLoadCompleted;
            }
        }

        public void LoadScene(string sceneName, SceneLoadOptions options = default)
        {
            var config = sceneTable.GetConfig(sceneName);

            var transitionIn = options.TransitionIn ?? config.transitionIn;
            var transitionOut = options.TransitionOut ?? config.transitionOut;

            loader.LoadSequenceAsync(sceneName, transitionIn, transitionOut, options.Payload).Forget();
        }

        private void HandleLoadStarted()
        {
            InputManager.Instance.LockInput();
        }

        private void HandleLoadFailed()
        {
            // todo ! UI 매니저 구현 필요
            // UI를 통해 메인 화면으로 이동 메시지 출력
            // 확인 누를 시 메인 화면으로 로드 시작
            // 이미 로딩화면인 상태일테니 바로 시작하도록 할 것
        }

        private void HandleLoadCompleted()
        {
            InputManager.Instance.UnlockInput();
        }
    }
}