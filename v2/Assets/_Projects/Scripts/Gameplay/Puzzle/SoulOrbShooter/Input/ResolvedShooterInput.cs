namespace MaouSuika.Gameplay
{
    public readonly struct ResolvedShooterInput
    {
        public readonly AimData? Aim;
        public readonly bool FireRequested;

        private ResolvedShooterInput(AimData? aim, bool fireRequested)
        {
            Aim = aim;
            FireRequested = fireRequested;
        }

        public static ResolvedShooterInput Idle() => new(null, false);

        public static ResolvedShooterInput Aiming(AimData aim) => new(aim, false);

        public static ResolvedShooterInput Fire(AimData aim) => new(aim, true);
    }
}