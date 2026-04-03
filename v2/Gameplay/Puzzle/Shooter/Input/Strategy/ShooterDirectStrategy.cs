using TBM.MaouSuika.Core;
using TBM.MaouSuika.Core.Settings;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    public class ShooterDirectStrategy : IShooterInputStrategy
    {
        public ShooterInputType InputType => ShooterInputType.Direct;

        private DirectAimConfig _config;

        public void Enter() { }

        public void Exit() { }

        public void UpdateConfig(InputProfile profile, string scheme)
        {
            _config = profile.GetDirectAimConfig(scheme);
        }

        public ShooterInputCommand Process(ShooterInputResult input, float deltaTime)
        {
            if (!input.IsAiming) return ShooterInputCommand.Idle();

            var aim = GetAdjustedAim(input.AimValue);
            var aimMagnitude = aim.magnitude;

            if (!IsValidAim(aim, aimMagnitude)) return ShooterInputCommand.Idle();

            var aimData = BuildAimData(aim, aimMagnitude);

            return input.FireRequestedThisFrame
                ? ShooterInputCommand.Fire(aimData)
                : ShooterInputCommand.Aim(aimData);
        }

        private Vector2 GetAdjustedAim(Vector2 aim)
        {
            if (_config.isSlingshotMode)
            {
                aim = -aim;
            }

            return aim;
        }

        private bool IsValidAim(Vector2 aim, float magnitude)
        {
            if (aim.y < 0f) return false;

            if (Mathf.Approximately(magnitude, 0f)) return false;

            return true;
        }

        private AimData BuildAimData(Vector2 aim, float magnitude)
        {
            var direction = aim / magnitude;
            var powerRatio = GameRule.Puzzle.Shooter.ClampFirePowerRatio(magnitude);
            return new AimData(direction, powerRatio);
        }
    }
}