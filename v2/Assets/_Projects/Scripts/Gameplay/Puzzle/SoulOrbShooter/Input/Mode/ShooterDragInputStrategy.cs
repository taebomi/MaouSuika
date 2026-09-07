using UnityEngine;

namespace MaouSuika.Gameplay
{
    public class ShooterDragInputStrategy : IShooterInputStrategy
    {
        private ShooterDragInputSettings _settings;

        private Vector2 _dragStartPosition;
        private bool _isDragging;

        public ShooterDragInputStrategy(ShooterDragInputSettings settings)
        {
            _settings = settings;
        }

        public void SetSettings(ShooterDragInputSettings settings)
        {
            _settings = settings;
        }

        public void Enter()
        {
            _isDragging = false;
        }

        public void Exit() { }

        public ResolvedShooterInput Resolve(in ShooterInputSnapshot input, float deltaTime)
        {
            if (input is { FirePressedThisFrame: true, Aim: { } startPosition })
            {
                _dragStartPosition = startPosition;
                _isDragging = true;
            }

            if (!_isDragging || input.Aim is not { } currentPosition) return ResolvedShooterInput.Idle();

            var diff = currentPosition - _dragStartPosition;

            if (_settings.isSlingshot) diff = -diff;

            if (!TryCreateAimData(diff, out var aimData))
            {
                if (input.FireReleasedThisFrame) _isDragging = false;
                return ResolvedShooterInput.Idle();
            }

            if (!input.FireReleasedThisFrame)
                return ResolvedShooterInput.Aiming(aimData);

            _isDragging = false;
            return ResolvedShooterInput.Fire(aimData);
        }

        private bool TryCreateAimData(Vector2 diff, out AimData aim)
        {
            var distance = diff.magnitude;
            var powerRatio = distance / _settings.dragRange;

            if (diff.y < 0f || powerRatio < ShooterRules.MIN_POWER_RATIO)
            {
                aim = default;
                return false;
            }

            aim = new AimData(diff / distance, ShooterRules.ClampPowerRatio(powerRatio));
            return true;
        }
    }
}