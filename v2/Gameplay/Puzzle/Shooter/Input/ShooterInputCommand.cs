namespace TBM.MaouSuika.Gameplay.Puzzle
{
    public readonly struct ShooterInputCommand
    {
        public enum CommandType
        {
            None,
            Idle,
            Aim,
            Fire,
        }
        public CommandType Type { get; init; }
        public AimData AimData { get; init; }

        public static ShooterInputCommand Idle() =>
            new()
            {
                Type = CommandType.Idle,
            };

        public static ShooterInputCommand Aim(AimData aimData) =>
            new()
            {
                Type = CommandType.Aim,
                AimData = aimData,
            };

        public static ShooterInputCommand Fire(AimData aimData) =>
            new()
            {
                Type = CommandType.Fire,
                AimData = aimData,
            };
    }
}