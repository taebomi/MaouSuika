using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MaouSuika.Core.Save;
using UnityEngine;

namespace MaouSuika.Core
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;
        private static UniTaskCompletionSource _initializedTcs = new();

        public static bool IsRegistered => _instance != null;
        public static bool IsInitialized => _initializedTcs.Task.Status is UniTaskStatus.Succeeded;

        [SerializeField] private AudioManager audioManager;
        [SerializeField] private InputManager input;
        [SerializeField] private SceneFlowManager scene;
        [SerializeField] private SaveManager save;
        [SerializeField] private SettingsManager settings;
        [SerializeField] private LocalizationManager localization;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            _instance = null;
            _initializedTcs = new UniTaskCompletionSource();
        }

        public static UniTask WaitUntilInitializedAsync(CancellationToken token)
        {
            return _initializedTcs.Task.AttachExternalCancellation(token);
        }


        private void Awake()
        {
            if (_instance != null && _instance != this)
                throw new InvalidOperationException($"{nameof(GameManager)} is already registered.");

            _instance = this;
        }

        private void Start()
        {
            if (!ReferenceEquals(_instance, this)) return;

            InitializeAsync(destroyCancellationToken).Forget();
        }

        private async UniTaskVoid InitializeAsync(CancellationToken token)
        {
            try
            {
                // Initialization 작업
                audioManager.Initialize();
                settings.Initialize();
                save.Initialize();
                input.Initialize();
                scene.Initialize();
                var settingsData = save.LoadSettings();
                await localization.InitializeAsync(settingsData.localizationSettings.localeCode, token);
                
                settings.Apply(settingsData);

                _initializedTcs.TrySetResult();
            }
            catch (OperationCanceledException ex)
            {
                _initializedTcs.TrySetCanceled(ex.CancellationToken);
            }
            catch (Exception ex)
            {
                _initializedTcs.TrySetException(ex);
            }

            await UniTask.CompletedTask;
        }
    }
}