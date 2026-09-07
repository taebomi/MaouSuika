using System;

namespace MaouSuika.Gameplay
{
    public class ShooterInputResolver
    {
        private readonly ShooterDirectInputStrategy _directInputStrategy;
        private readonly ShooterDragInputStrategy _dragInputStrategy;

        private IShooterInputStrategy _currentStrategy;

        public ShooterControlMode ControlMode { get; private set; }

        public ShooterInputResolver(ShooterControlSchemeSettings settings)
        {
            _dragInputStrategy = new ShooterDragInputStrategy(settings.drag);
            _directInputStrategy = new ShooterDirectInputStrategy(settings.direct);

            ActivateControlMode(settings.controlMode);
        }

        private void ActivateControlMode(ShooterControlMode mode)
        {
            ControlMode = mode;
            _currentStrategy = GetStrategy(mode);
            _currentStrategy.Enter();
        }

        public void SetControlMode(ShooterControlMode controlMode)
        {
            if (ControlMode == controlMode) return;

            var nextStrategy = GetStrategy(controlMode);

            _currentStrategy.Exit();

            ControlMode = controlMode;
            _currentStrategy = nextStrategy;

            _currentStrategy.Enter();
        }

        public void Reset()
        {
            _currentStrategy.Exit();
            _currentStrategy.Enter();
        }

        public void ApplySettings(ShooterControlSchemeSettings settings)
        {
            _currentStrategy.Exit();

            _directInputStrategy.SetSettings(settings.direct);
            _dragInputStrategy.SetSettings(settings.drag);

            ActivateControlMode(settings.controlMode);
        }

        private IShooterInputStrategy GetStrategy(ShooterControlMode controlMode) =>
            controlMode switch
            {
                ShooterControlMode.Direct => _directInputStrategy,
                ShooterControlMode.Drag => _dragInputStrategy,
                _ => throw new NotSupportedException($"{controlMode} is not supported."),
            };

        public ResolvedShooterInput Resolve(in ShooterInputSnapshot input, float deltaTime)
        {
            return _currentStrategy.Resolve(in input, deltaTime);
        }
    }
}