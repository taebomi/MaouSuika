namespace MaouSuika.Core
{
    public static class MenuPath
    {
        private const string ROOT = "Maou Suika/";

        public static class Gameplay
        {
            private const string GAMEPLAY_ROOT = ROOT + "Gameplay/";
            public const string POP_EFFECT = GAMEPLAY_ROOT + "Pop Effect/";
            public const string SOUL_ORB = GAMEPLAY_ROOT + "Soul Orb/";
            public const string SCORE = GAMEPLAY_ROOT + "Score/";
            public const string COMBO = GAMEPLAY_ROOT + "Combo/";
            public const string GAMEOVER = GAMEPLAY_ROOT + "GameOver/";
            public const string SHOOTER = GAMEPLAY_ROOT + "Shooter/";
            public const string COMBAT = GAMEPLAY_ROOT + "Combat/";
            public const string Region = GAMEPLAY_ROOT + "Region/";
            public const string SKILL = GAMEPLAY_ROOT + "Skill/";


            public static class Unit
            {
                private const string UNIT_ROOT = GAMEPLAY_ROOT + "Unit/";

                public const string MONSTER = UNIT_ROOT + "Monster/";
            }
        }
    }
}