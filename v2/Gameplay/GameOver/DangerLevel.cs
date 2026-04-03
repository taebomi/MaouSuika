namespace TBM.MaouSuika.Gameplay.Puzzle
{
    public enum DangerLevel
    {
        None,
        Safe,
        Warning,
        Critical,
        GameOver,
    }

    public static class DangerLevelExtensions
    {
        public static float ToFmodLabel(this DangerLevel level)
        {
            return level switch
            {
                DangerLevel.Safe => 0f,
                DangerLevel.Warning => 1f,
                DangerLevel.Critical => 2f,
                _ => 0f,
            };
        }
    }
}