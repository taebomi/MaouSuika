using TBM.MaouSuika.Core;
using TBM.MaouSuika.Core.Settings;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    public class ShooterDragStrategy : IShooterInputStrategy
    {
        private readonly PuzzleArea _area;

        private DragAimConfig _config;
        private Vector2 _startPos;
        private bool _isDragging;

        private AimData _cachedAimData;

        public ShooterInputType InputType => ShooterInputType.Drag;

        public ShooterDragStrategy(PuzzleArea area)
        {
            _area = area;
        }

        public void Enter()
        {
            _isDragging = false;
        }

        public void Exit() { }

        public void UpdateConfig(InputProfile profile, string scheme)
        {
            _config = profile.GetDragConfig(scheme);
        }

        public ShooterInputCommand Process(ShooterInputResult input, float deltaTime)
        {
            if (input.FireRequestedThisFrame && _isDragging)
            {
                _isDragging = false;
                return ShooterInputCommand.Fire(_cachedAimData);
            }

            if (!input.IsAiming)
            {
                _isDragging = false;
                return ShooterInputCommand.Idle();
            }

            var diff = GetDragDiff(input.AimValue);
            if (!IsValidDrag(diff)) return ShooterInputCommand.Idle();

            var aimData = BuildAimData(diff);
            _cachedAimData = aimData;
            return ShooterInputCommand.Aim(aimData);
        }

        private Vector2 GetDragDiff(Vector2 screenPos)
        {
            if (!_isDragging)
            {
                _startPos = _area.ScreenToWorldPoint(screenPos);
                _isDragging = true;
            }

            var curPos = _area.ScreenToWorldPoint(screenPos);
            var diff = curPos - _startPos;
            return _config.isSlingshotMode ? -diff : diff;
        }

        private bool IsValidDrag(Vector2 diff)
        {
            if (diff.y < 0f) return false;

            var powerRatio = diff.magnitude / _config.dragRange;
            if (powerRatio < GameRule.Puzzle.Shooter.MIN_FIRE_POWER_RATIO) return false;

            return true;
        }

        private AimData BuildAimData(Vector2 diff)
        {
            var dist = diff.magnitude;
            var dir = diff / dist;
            var power = GameRule.Puzzle.Shooter.ClampFirePowerRatio(dist / _config.dragRange);
            return new AimData(dir, power);
        }
    }
}