namespace TBM.MaouSuika.Gameplay.Puzzle
{
    public enum ComboGrade
    {
        None,
        Low,
        Mid,
        High,
        Extreme,
    }

    public static class ComboConfig
    {
        public const int LOW = 3;
        public const int MID = 7;
        public const int HIGH = 11;
        public const int EXTREME = 15;
    }

    public static class ComboUtility
    {
        public static ComboGrade ConvertFrom(int combo)
        {
            return combo switch
            {
                >= ComboConfig.EXTREME => ComboGrade.Extreme,
                >= ComboConfig.HIGH => ComboGrade.High,
                >= ComboConfig.MID => ComboGrade.Mid,
                >= ComboConfig.LOW => ComboGrade.Low,
                _ => ComboGrade.None,
            };
        }

        public static bool IsCombo(int combo) => combo >= ComboConfig.LOW;
    }
}