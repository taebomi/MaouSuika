using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TBM.MaouSuika.Core.Save;
using TBM.MaouSuika.Core.Settings;
using UnityEngine;
using UnityEngine.InputSystem;
using InputSettings = TBM.MaouSuika.Core.Settings.InputSettings;

namespace TBM.MaouSuika.Core.Input
{
    public class InputManager : CoreManager<InputManager>, ISettingsDataHandler
    {
        [SerializeField] private PlayerInputManager playerInputManager;

        [ShowInInspector] public InputSettings CurrentSettings { get; private set; }
        public InputController MainPlayer => _playerControllers[0];

        private List<InputProfile> Profiles => CurrentSettings.profiles;

        private readonly List<InputController> _playerControllers = new();


        public void Initialize(InputSettings settings)
        {
            CurrentSettings = settings == null ? new InputSettings() : new InputSettings(settings);
            playerInputManager.JoinPlayer();
        }

        public void SwitchMap(string mapName)
        {
            foreach (var playerController in _playerControllers)
            {
                playerController.SwitchMap(mapName);
            }
        }

        public InputController GetInputController(int playerIndex)
        {
            if (playerIndex < 0 || playerIndex >= _playerControllers.Count)
            {
                Logger.Error($"Player[{playerIndex}] is not found.");
                return null;
            }

            var controller = _playerControllers.FirstOrDefault(controller => controller.PlayerIndex == playerIndex);
            if (controller == null)
            {
                Logger.Error($"Player[{playerIndex}] Input Controller is not found.");
                return null;
            }

            return controller;
        }

        public void LockInput()
        {
            // 입력 제한
        }

        public void UnlockInput()
        {
            // 입력 제한 해제
        }

        public void OnPlayerJoined(PlayerInput input)
        {
            SetupPlayer(input);
        }

        public void OnPlayerLeft(PlayerInput input)
        {
            TeardownPlayer(input);
        }

        private void SetupPlayer(PlayerInput input)
        {
            var controller = input.GetComponent<InputController>();
            if (_playerControllers.Contains(controller))
            {
                Logger.Warning($"왜 있지?");
                return;
            }

            while (Profiles.Count <= controller.PlayerIndex) // 없을 시 기본값 추가
            {
                Profiles.Add(new InputProfile());
            }

            controller.SetProfile(Profiles[controller.PlayerIndex]);
            controller.transform.SetParent(transform);
            controller.name = $"Player {controller.PlayerIndex}";

            _playerControllers.Add(controller);
        }

        private void TeardownPlayer(PlayerInput input)
        {
            var controller = input.GetComponent<InputController>();
            if (!_playerControllers.Contains(controller))
            {
                Logger.Warning($"왜 없지?");
                return;
            }

            _playerControllers.Remove(controller);
        }

        #region Save / Load

        public void OnSaveData(SettingsData data)
        {
            data.input = new InputSettings(CurrentSettings);
        }

        public void OnLoadData(SettingsData data)
        {
            CurrentSettings = data.input != null
                ? new InputSettings(data.input)
                : new InputSettings();

            // Check Reliability
            CurrentSettings.profiles ??= new List<InputProfile>();

            // Update Profile
            foreach (var controller in _playerControllers)
            {
                while (Profiles.Count < controller.PlayerIndex)
                {
                    Profiles.Add(new InputProfile());
                }

                controller.SetProfile(Profiles[controller.PlayerIndex]);
            }
        }

        #endregion
    }
}