using System;
using Sirenix.OdinInspector;
using TBM.MaouSuika.Core.Settings;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TBM.MaouSuika.Core.Input
{
    public class InputController : MonoBehaviour
    {
        [SerializeField] private PlayerInput playerInput;

        [ShowInInspector] public int PlayerIndex => playerInput.playerIndex;
        [ShowInInspector] public InputProfile Profile { get; private set; }

        public string Scheme => playerInput.currentControlScheme;

        public event Action<string> ControlSchemeChanged;
        public event Action<InputProfile> ProfileChanged;

        private void Awake()
        {
            playerInput.onControlsChanged += OnControlsChanged;
        }

        private void OnDestroy()
        {
            if (playerInput != null) playerInput.onControlsChanged -= OnControlsChanged;
        }


        public void SetProfile(InputProfile profile)
        {
            Profile = profile;
            NotifyProfileChanged();
        }

        public void SwitchMap(string mapName)
        {
            playerInput.SwitchCurrentActionMap(mapName);
        }

        public void NotifyProfileChanged()
        {
            ProfileChanged?.Invoke(Profile);
        }

        public InputAction GetAction(string mapName, string actionName)
        {
            var map = playerInput.actions.FindActionMap(mapName);
            if (map == null)
            {
                Logger.Error($"Cannot find action map[{mapName}]");
                return null;
            }

            var action = map.FindAction(actionName);
            if (action == null)
            {
                Logger.Error($"Cannot find action[{actionName}] in map[{mapName}]");
                return null;
            }

            return action;
        }

        public void OnControlsChanged(PlayerInput _)
        {
            Logger.Info($"Controls Changed !\n" +
                        $"Control Scheme : {Scheme}");
            ControlSchemeChanged?.Invoke(Scheme);
        }
    }
}