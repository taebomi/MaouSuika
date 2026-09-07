using System;
using MaouSuika.Gameplay;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MaouSuika.Core
{
    public class LocalPlayerInput : MonoBehaviour
    {
        [SerializeField] private PlayerInput playerInput;

        private readonly InputProfile _profile = InputProfile.CreateDefault();

        private readonly PlayerHaptics _haptics = new();

        private readonly Subject<string> _controlSchemeChanged = new();
        private readonly Subject<Unit> _inputProfileChanged = new();

        public Observable<string> ControlSchemeChanged => _controlSchemeChanged;
        public Observable<Unit> InputProfileChanged => _inputProfileChanged;

        public InputProfile Profile => _profile;
        public int PlayerIndex => playerInput.playerIndex;
        public string CurrentControlScheme => playerInput.currentControlScheme;

        private void OnEnable()
        {
            playerInput.onControlsChanged += OnControlsChanged;
        }

        private void OnDisable()
        {
            playerInput.onControlsChanged -= OnControlsChanged;
        }

        private void OnDestroy()
        {
            _controlSchemeChanged.Dispose();
            _inputProfileChanged.Dispose();
        }

        public void ApplyInputProfile(InputProfile source)
        {
            _profile.CopyFrom(source);
            _inputProfileChanged.OnNext(Unit.Default);
        }

        public InputActionMap GetActionMap(string mapName)
        {
            var map = playerInput.actions.FindActionMap(mapName);

            if (map == null)
            {
                Debug.LogError($"Cannot Find Map [{mapName}]");
                return null;
            }

            return map;
        }

        public void SetControlMode(ShooterControlMode mode)
        {
            Profile.GetSettings(CurrentControlScheme).controlMode = mode;
            _inputProfileChanged.OnNext(Unit.Default);
        }

        public InputAction GetAction(string mapName, string actionName)
        {
            var map = GetActionMap(mapName);
            if (map == null) return null;

            var action = map.FindAction(actionName);
            if (action == null)
            {
                Debug.LogError($"Cannot find action [{actionName}] in map [{mapName}]");
                return null;
            }

            return action;
        }

        public void RequestHaptics(HapticRequest request)
        {
            _haptics.Request(request);
        }

        private void OnControlsChanged(PlayerInput _)
        {
            _controlSchemeChanged.OnNext(CurrentControlScheme);
        }
    }
}