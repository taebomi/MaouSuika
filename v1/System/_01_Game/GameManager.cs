using Cysharp.Threading.Tasks;
using SOSG.System.Dialogue;
using SOSG.System.Audio;
using SOSG.System.Cheat;
using SOSG.System.Display;
using SOSG.System.Input;
using SOSG.System.Localization;
using SOSG.System.PlayData;
using SOSG.System.Scene;
using SOSG.System.Setting;
using SOSG.System.UI;
using SOSG.System.Vibration;
using TaeBoMi;
using UnityEngine;

namespace SOSG.System
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager instance;

        [field: SerializeField] public SceneLoadController SceneLoadController { get; private set; }
        [field: SerializeField] public SceneSetUpTasker SceneSetUpTasker { get; private set; }
        [field: SerializeField] public InputManager InputManager { get; private set; }

        [field: SerializeField] public UIManager UIManager { get; private set; }

        [SerializeField] private PlayDataManager playDataManager;

        [SerializeField] private SettingDataManager settingDataManager;
        [SerializeField] private AudioManager audioManager;
        [SerializeField] private VibrationManager vibrationManager;
        [SerializeField] private DisplayManager displayManager;

        [field: SerializeField] public LocalizationManager LocalizationManager { get; private set; }
        [field: SerializeField] public DialogueManager DialogueManager { get; private set; }

        private bool _isSetUpCompleted;

        public static GameManager Instance
        {
            get
            {
                if (ReferenceEquals(instance, null))
                {
                    if (Application.isPlaying is false)
                    {
                        return null;
                    }

                    instance = GetOrCreateGameManager();
                }

                return instance;
            }
            private set => instance = value;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void InitializeOnSubsystemRegistration()
        {
            Instance = null;
        }

        private static GameManager GetOrCreateGameManager()
        {
            var gameManager = FindFirstObjectByType<GameManager>();
            if (ReferenceEquals(gameManager, null) is false)
            {
                return gameManager;
            }

            var gameManagerPrefab = Resources.Load<GameManager>("Game Manager");
            if (gameManagerPrefab is null)
            {
                TBMUtility.LogError(
                    "# GameSetUpHelper - InitializeBeforeSceneLoad - Game Manager Prefab is not found.");
                return null;
            }

            gameManager = Instantiate(gameManagerPrefab);
            gameManager.transform.name = "Game Manager";
            return gameManager;
        }

        public static async UniTask WaitForSetUpCompleted()
        {
            if (Instance._isSetUpCompleted)
            {
                return;
            }

            await UniTask.WaitUntil(() => Instance._isSetUpCompleted,
                cancellationToken: Application.exitCancellationToken);
        }


        private void Awake()
        {
            SetUpAsync().Forget();
        }

        private async UniTaskVoid SetUpAsync()
        {
            settingDataManager.SetUp();
            audioManager.ApplySetting(SettingDataHelper.AudioSetting);
            vibrationManager.SetUp();
            displayManager.SetUp();

            await LocalizationManager.SetUpAsync(SettingDataHelper.InterfaceSetting);
            await playDataManager.SetUpAsync();

            _isSetUpCompleted = true;
        }

        private void OnDestroy()
        {
            vibrationManager.TearDown();
            displayManager.TearDown();
            settingDataManager.TearDown();
        }
    }
}