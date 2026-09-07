using MaouSuika.Gameplay.Shooter;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MaouSuika.Gameplay
{
    public class GamepadShooterInputReader : IShooterInputReader
    {
        private readonly InputAction _aim;
        private readonly InputAction _fire;

        public GamepadShooterInputReader(InputAction aim, InputAction fire)
        {
            _aim = aim;
            _fire = fire;
        }

        public ShooterInputSnapshot ReadInput()
        {
            var aim = _aim.ReadValue<Vector2>();
            return new ShooterInputSnapshot(
                aim,
                _fire.WasPressedThisFrame(),
                _fire.IsPressed(),
                _fire.WasReleasedThisFrame());
        }
    }
}