namespace MaouSuika.Gameplay
{
    public enum ComboGrade
    {
        None,
        Normal,
        High,
        Max,
    }

    public static class ComboGrades
    {
        public const int NORMAL_MIN_COMBO = 3;
        public const int HIGH_MIN_COMBO = 8;
        public const int MAX_MIN_COMBO = 15;

        public static ComboGrade GradeOf(int combo)
        {
            return combo switch
            {
                < NORMAL_MIN_COMBO => ComboGrade.None,
                < HIGH_MIN_COMBO => ComboGrade.Normal,
                < MAX_MIN_COMBO => ComboGrade.High,
                _ => ComboGrade.Max,
            };
        }
    }
}