namespace TBM.MaouSuika.Gameplay.Puzzle
{
    [System.Flags]
    public enum ShooterState
    {
        None = 0,
        Blocked = 1 << 0,
        Cooldown = 1 << 1,
    }
}