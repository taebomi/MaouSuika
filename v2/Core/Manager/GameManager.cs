using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TBM.MaouSuika.Core.Audio;
using TBM.MaouSuika.Core.Input;
using TBM.MaouSuika.Core.Localization;
using TBM.MaouSuika.Core.Save;
using TBM.MaouSuika.Core.Scene;
using TBM.MaouSuika.Core.UGS;
using TBM.MaouSuika.Core.Vibration;
using UnityEngine;

namespace TBM.MaouSuika.Core
{
    [DefaultExecutionOrder(-1)]
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetSingleton()
        {
            Instance = null;
            SaveManager.ResetSingleton();
            InputManager.ResetSingleton();
            Vibration.VibrationManager.ResetSingleton();
            AudioManager.ResetSingleton();
            SceneManager.ResetSingleton();
            LocalizationManager.ResetSingleton();
            UGSManager.ResetSingleton();
        }

        [field: SerializeField] public SaveManager SaveManager { get; private set; }
        [field: SerializeField] public InputManager InputManager { get; private set; }
        [field: SerializeField] public VibrationManager VibrationManager { get; private set; }
        [field: SerializeField] public SceneManager SceneManager { get; private set; }
        [field: SerializeField] public AudioManager AudioManager { get; private set; }
        [field: SerializeField] public LocalizationManager LocalizationManager { get; private set; }
        [field: SerializeField] public UGSManager UGSManager { get; private set; }

        public async UniTask InitializeAsync(CancellationToken token)
        {
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token, destroyCancellationToken);

            DontDestroyOnLoad(gameObject);

            await InitializeInternalAsync(linkedCts.Token);

            RegisterSettingsHandlers();
            RegisterManagers();
        }

        private async UniTask InitializeInternalAsync(CancellationToken token)
        {
            var settingsData = SaveManager.LoadSettingsData();

            await AudioManager.InitializeAsync(settingsData.audio);
            await LocalizationManager.InitializeAsync();
            InputManager.Initialize(settingsData.input);

            // await UGSManager.InitializeAsync();
        }

        private void RegisterSettingsHandlers()
        {
            SaveManager.RegisterSettingsHandler(AudioManager);
            SaveManager.RegisterSettingsHandler(InputManager);
        }

        private void RegisterManagers()
        {
            Instance = this;
            SaveManager.RegisterSingleton();
            InputManager.RegisterSingleton();
            VibrationManager.RegisterSingleton();
            SceneManager.RegisterSingleton();
            AudioManager.RegisterSingleton();
            LocalizationManager.RegisterSingleton();
            UGSManager.RegisterSingleton();
        }
    }
}