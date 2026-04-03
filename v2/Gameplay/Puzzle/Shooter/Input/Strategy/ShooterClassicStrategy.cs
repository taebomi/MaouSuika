using TBM.MaouSuika.Core;
using TBM.MaouSuika.Core.Settings;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    public class ShooterClassicStrategy : IShooterInputStrategy
    {
        private const float MIN_ANGLE = -90f;
        private const float MAX_ANGLE = 90f;

        private ClassicAimConfig _config;

        private float _curAngle;
        private float _curPowerRatio;

        public ShooterInputType InputType => ShooterInputType.Classic;

        public void Enter()
        {
            _curAngle = 0f;
            _curPowerRatio = 0.5f;
        }

        public void Exit() { }

        public void UpdateConfig(InputProfile profile, string scheme)
        {
            _config = profile.GetMoveConfig(scheme);
        }

        public ShooterInputCommand Process(ShooterInputResult input, float deltaTime)
        {
            var aimValue = input.AimValue;

            UpdateAngle(aimValue.x, deltaTime);
            UpdatePowerRatio(aimValue.y, deltaTime);

            var aimData = BuildAimData();

            return input.FireRequestedThisFrame
                ? ShooterInputCommand.Fire(aimData)
                : ShooterInputCommand.Aim(aimData);
        }

        private void UpdateAngle(float inputX, float deltaTime)
        {
            if (Mathf.Approximately(inputX, 0f)) return;

            _curAngle += inputX * _config.angleChangeSpeed * deltaTime;
            _curAngle = Mathf.Clamp(_curAngle, MIN_ANGLE, MAX_ANGLE);
        }

        private void UpdatePowerRatio(float inputY, float deltaTime)
        {
            if (Mathf.Approximately(inputY, 0f)) return;
            _curPowerRatio += inputY * _config.powerChangeSpeed * deltaTime;
            _curPowerRatio = GameRule.Puzzle.Shooter.ClampFirePowerRatio(_curPowerRatio);
        }

        private AimData BuildAimData()
        {
            var rad = _curAngle * Mathf.Deg2Rad;
            var direction = new Vector2(Mathf.Sin(rad), Mathf.Cos(rad));
            return new AimData(direction, _curPowerRatio);
        }
    }
}