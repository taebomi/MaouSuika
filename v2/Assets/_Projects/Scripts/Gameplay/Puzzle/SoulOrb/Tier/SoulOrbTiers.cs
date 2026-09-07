namespace MaouSuika.Gameplay
{
    public static class SoulOrbTiers
    {
        public const int MAX_TIER = 10;
        public const int COUNT = MAX_TIER + 1;
        public const int GRADE_COUNT = 4;

        public static Grade GradeOf(int tier)
        {
            return tier switch
            {
                <= 4 => Grade.Common,
                <= 7 => Grade.Rare,
                <= 9 => Grade.Epic,
                _ => Grade.Legendary,
            };
        }
    }
}