using System;
using System.Collections.Generic;
using MaouSuika.Gameplay;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MaouSuika.Core
{
    [RequireComponent(typeof(PlayerInputManager))]
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance { get; private set; }

        private PlayerInputManager _playerInputManager;
        private readonly Dictionary<int, LocalPlayerInput> _players = new();

        public LocalPlayerInput MainPlayer { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            Instance = null;
        }

        private void Awake()
        {
            _playerInputManager = GetComponent<PlayerInputManager>();
        }

        private void OnEnable()
        {
            _playerInputManager.onPlayerJoined += OnPlayerJoined;
            _playerInputManager.onPlayerLeft += OnPlayerLeft;
        }

        public void Initialize()
        {
            Instance = this;

            _playerInputManager.JoinPlayer();
            _playerInputManager.DisableJoining();
        }

        private void OnDisable()
        {
            _playerInputManager.onPlayerJoined -= OnPlayerJoined;
            _playerInputManager.onPlayerLeft -= OnPlayerLeft;
        }

        private void OnDestroy()
        {
            if (!ReferenceEquals(Instance, this)) return;

            Instance = null;
        }

        public bool TryGetPlayer(int playerIndex, out LocalPlayerInput player)
        {
            return _players.TryGetValue(playerIndex, out player);
        }

        private void OnPlayerJoined(PlayerInput playerInput)
        {
            var localPlayerInput = playerInput.GetComponent<LocalPlayerInput>();
            RegisterPlayer(localPlayerInput);

            MainPlayer ??= localPlayerInput;
        }

        private void OnPlayerLeft(PlayerInput playerInput)
        {
            var localPlayerInput = playerInput.GetComponent<LocalPlayerInput>();
            UnregisterPlayer(localPlayerInput);
        }

        private void RegisterPlayer(LocalPlayerInput localPlayerInput)
        {
            if (!_players.TryAdd(localPlayerInput.PlayerIndex, localPlayerInput))
                throw new InvalidOperationException($"왜 있지?");

            localPlayerInput.transform.SetParent(transform);
            localPlayerInput.name = $"Player {localPlayerInput.PlayerIndex}";
        }

        private void UnregisterPlayer(LocalPlayerInput localPlayerInput)
        {
            if (!_players.Remove(localPlayerInput.PlayerIndex))
                throw new InvalidOperationException($"왜 없지?");
        }

        public InputProfile CaptureSettings()
        {
            return MainPlayer.Profile.Copy();
        }
    }
}