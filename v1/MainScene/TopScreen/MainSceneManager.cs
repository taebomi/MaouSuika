using SOSG.System.Scene;
using UnityEngine;

namespace SOSG.MainScene
{
    public class MainSceneManager : MonoBehaviour
    {
#if SOSG_DEBUG
        [SerializeField] private bool skipIntroOption;
#endif
        [SerializeField] private BgmController bgmController;

        public static bool SkipIntro;


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize()
        {
            SkipIntro = false;
        }

        private void Awake()
        {
#if SOSG_DEBUG
            SkipIntro = skipIntroOption;
#endif
            SceneSetUpHelper.Completed += OnSceneSetUpCompleted;
        }

        private void OnSceneSetUpCompleted()
        {
            bgmController.PlayBgm(SkipIntro);
        }
    }
}