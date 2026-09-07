using System;
using MaouSuika.UI;
using UnityEngine;
using UnityEngine.UI;

namespace MaouSuika.Gameplay
{
    public class PauseUIController : MonoBehaviour
    {
        [SerializeField] private GameObject pauseScreen;
        [SerializeField] private SettingsScreenController settingsScreen;

        [Header("Buttons")]
        [SerializeField] private Button pauseButton;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button settingsButton;

        private Action _pauseRequested;
        private Action _resumeRequested;
        private Action _restartRequested;

        private void Awake()
        {
            pauseButton.onClick.AddListener(OnPauseRequested);
            resumeButton.onClick.AddListener(OnResumeRequested);
            settingsButton.onClick.AddListener(ShowSettingsScreen);
            restartButton.onClick.AddListener(OnRestartRequested);

            settingsScreen.Hide();
            pauseButton.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            pauseButton.onClick.RemoveListener(OnPauseRequested);
            resumeButton.onClick.RemoveListener(OnResumeRequested);
            settingsButton.onClick.RemoveListener(ShowSettingsScreen);
            restartButton.onClick.RemoveListener(OnRestartRequested);
        }

        public void Initialize(Action pauseRequested, Action resumeRequested, Action restartRequested)
        {
            _pauseRequested = pauseRequested;
            _resumeRequested = resumeRequested;
            _restartRequested = restartRequested;

            settingsScreen.Initialize(CloseSettings);
        }

        private void ShowSettingsScreen()
        {
            settingsScreen.Show();
        }

        private void CloseSettings()
        {
            settingsScreen.Hide();
        }

        public void ShowPlaying()
        {
            settingsScreen.Hide();
            pauseScreen.SetActive(false);
            pauseButton.gameObject.SetActive(true);
        }

        public void ShowPaused()
        {
            settingsScreen.Hide();
            pauseScreen.SetActive(true);
            pauseButton.gameObject.SetActive(false);
        }

        public bool TryHandleBack()
        {
            return settingsScreen.TryHandleBack();
        }

        public void Hide()
        {
            settingsScreen.Hide();
            pauseScreen.SetActive(false);
            pauseButton.gameObject.SetActive(false);
        }

        private void OnPauseRequested()
        {
            _pauseRequested?.Invoke();
        }

        private void OnRestartRequested()
        {
            _restartRequested?.Invoke();
        }

        private void OnResumeRequested()
        {
            _resumeRequested?.Invoke();
        }
    }
}
