using System;
using TBM.MaouSuika.Core.Input;
using TBM.MaouSuika.Core.Settings;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    public class ShooterInputHandler
    {
        private readonly InputAction _aimAction;
        private readonly InputAction _fireAction;

        private ShooterInputType _currentInputType;
        private float _deadZone;

        public ShooterInputHandler(InputController inputController)
        {
            const string map = InputConstants.Map.GAMEPLAY;

            _aimAction = inputController.GetAction(map, InputConstants.Gameplay.AIM);
            _fireAction = inputController.GetAction(map, InputConstants.Gameplay.FIRE);
        }

        public void SetInputType(ShooterInputType inputType)
        {
            _currentInputType = inputType;
        }

        public void SetDeadZone(float deadZone)
        {
            _deadZone = deadZone;
        }

        public ShooterInputResult ReadInput()
        {
            var aimValue = _aimAction.ReadValue<Vector2>();
            var isAiming = _aimAction.IsPressed();

            ApplyDeadZone(ref aimValue, ref isAiming);

            return new ShooterInputResult
            {
                InputType = _currentInputType,
                AimValue = aimValue,
                AimStartedThisFrame = _aimAction.WasPressedThisFrame(),
                IsAiming = isAiming,
                FireRequestedThisFrame = _fireAction.WasPerformedThisFrame(),
            };
        }

        private void ApplyDeadZone(ref Vector2 aimValue, ref bool isAiming)
        {
            if (_deadZone > 0f && aimValue.magnitude < _deadZone)
            {
                aimValue = Vector2.zero;
                isAiming = false;
            }
        }
    }
}