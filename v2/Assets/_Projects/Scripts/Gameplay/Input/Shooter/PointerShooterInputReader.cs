using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MaouSuika.Gameplay.Shooter
{
    public class PointerShooterInputReader : IShooterInputReader
    {
        private readonly InputAction _aim;
        private readonly InputAction _fire;

        private readonly Func<Vector2, Vector2> _screenToWorld;
        private readonly Func<bool> _isPointerOverUI;

        private Vector2? _lastAim;
        private bool _blockedByUI;

        public PointerShooterInputReader(InputAction aim, InputAction fire, Func<Vector2, Vector2> screenToWorld,
            Func<bool> isPointerOverUI)
        {
            _aim = aim;
            _fire = fire;
            _screenToWorld = screenToWorld;
            _isPointerOverUI = isPointerOverUI;
        }

        public ShooterInputSnapshot ReadInput()
        {
            var pressed = _fire.WasPressedThisFrame();
            var held = _fire.IsPressed();
            var released = _fire.WasReleasedThisFrame();

            if (pressed)
                _blockedByUI = _isPointerOverUI();

            if (_blockedByUI)
            {
                _lastAim = null;
                if (released)
                    _blockedByUI = false;

                return default;
            }

            if (held)
            {
                var screenPosition = _aim.ReadValue<Vector2>();
                _lastAim = _screenToWorld(screenPosition);
            }

            var aim = pressed || held || released
                ? _lastAim
                : null;

            if (released)
                _lastAim = null;

            return new ShooterInputSnapshot(aim, pressed, held, released);
        }
    }
}