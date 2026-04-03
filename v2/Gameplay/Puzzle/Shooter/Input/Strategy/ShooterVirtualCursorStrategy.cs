using TBM.MaouSuika.Core;
using TBM.MaouSuika.Core.Settings;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    public class ShooterVirtualCursorStrategy : IShooterInputStrategy
    {
        public ShooterInputType InputType => ShooterInputType.VirtualCursor;

        private VirtualCursorAimConfig _config;
        private Vector2 _aimVector;

        public void Enter()
        {
            _aimVector = AimData.Default.Direction * AimData.Default.PowerRatio;
        }

        public void Exit() { }

        public void UpdateConfig(InputProfile profile, string scheme)
        {
            _config = profile.GetVirtualCursorAimConfig(scheme);
        }

        public ShooterInputCommand Process(ShooterInputResult input, float deltaTime)
        {
            if (input.IsAiming) ApplyInput(input.AimValue, deltaTime);

            var aimData = ToAimData();
            return input.FireRequestedThisFrame
                ? ShooterInputCommand.Fire(aimData)
                : ShooterInputCommand.Aim(aimData);
        }

        private void ApplyInput(Vector2 moveDelta, float deltaTime)
        {
            var newAimVector = _aimVector + moveDelta * (_config.cursorMoveSpeed * deltaTime);
            _aimVector = ConstrainAimVector(newAimVector);
        }

        private Vector2 ConstrainAimVector(Vector2 aimVector)
        {
            if (aimVector.y < 0f) aimVector.y = 0f;

            var magnitude = aimVector.magnitude;

            if (Mathf.Approximately(magnitude, 0f)) return _aimVector;

            var clampedMagnitude = GameRule.Puzzle.Shooter.ClampFirePowerRatio(magnitude);
            return (aimVector / magnitude) * clampedMagnitude;
        }

        private AimData ToAimData()
        {
            var magnitude = _aimVector.magnitude;
            if (Mathf.Approximately(magnitude, 0f)) return AimData.Default;

            var direction = _aimVector / magnitude;
            return new AimData(direction, magnitude);
        }
    }
}