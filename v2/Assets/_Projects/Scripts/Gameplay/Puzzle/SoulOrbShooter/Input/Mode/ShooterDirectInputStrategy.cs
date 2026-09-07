namespace MaouSuika.Gameplay
{
    public class ShooterDirectInputStrategy : IShooterInputStrategy
    {
        private ShooterDirectInputSettings _settings;

        public ShooterDirectInputStrategy(ShooterDirectInputSettings settings)
        {
            _settings = settings;
        }

        public void Enter() { }
        public void Exit() { }

        public void SetSettings(ShooterDirectInputSettings settings)
        {
            _settings = settings;
        }

        public ResolvedShooterInput Resolve(in ShooterInputSnapshot input, float deltaTime)
        {
            if (input.Aim is not { } aim) return ResolvedShooterInput.Idle();

            if (_settings.isSlingshot) aim = -aim;
            var magnitude = aim.magnitude;

            if (aim.y < 0f || magnitude < ShooterRules.MIN_POWER_RATIO) return ResolvedShooterInput.Idle();

            var aimData = new AimData(aim / magnitude, ShooterRules.ClampPowerRatio(magnitude));

            return input.FirePressedThisFrame
                ? ResolvedShooterInput.Fire(aimData)
                : ResolvedShooterInput.Aiming(aimData);
        }
    }
}