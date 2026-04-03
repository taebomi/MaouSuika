using TBM.MaouSuika.Gameplay.Puzzle;
using TBM.MaouSuika.Gameplay.Unit;
using UnityEngine;

namespace TBM.MaouSuika.Core
{
    public static class GameRule
    {
        public static class Monster { }

        public static class Puzzle
        {
            public static class Suika
            {
                public const int MIN_TIER = 0;
                public const int MAX_TIER = 10;
                public const int TIER_COUNT = MAX_TIER - MIN_TIER + 1;

                public const int COMMON_TIER = 3;
                public const int UNCOMMON_TIER = 6;
                public const int RARE_TIER = 9;
                public const int EPIC_TIER = 10;

                public static MonsterGrade ToMonsterGrade(int tier)
                {
                    return tier switch
                    {
                        <= COMMON_TIER => MonsterGrade.Common,
                        <= UNCOMMON_TIER => MonsterGrade.Uncommon,
                        <= RARE_TIER => MonsterGrade.Rare,
                        _ => MonsterGrade.Epic,
                    };
                }

                public static MergeEffectGrade ToMergeEffectGrade(int nextTier)
                {
                    return nextTier switch
                    {
                        <= COMMON_TIER => MergeEffectGrade.Common,
                        <= UNCOMMON_TIER => MergeEffectGrade.Uncommon,
                        <= RARE_TIER => MergeEffectGrade.Rare,
                        <= EPIC_TIER => MergeEffectGrade.Epic,
                        _ => MergeEffectGrade.Legendary,
                    };
                }
            }

            public static class Queue
            {
                public const int SIZE = 10;
                public const int DEFAULT_VISIBLE_COUNT = 1;
                public const int MAX_VISIBLE_COUNT = 5;

                public const int MIN = 0;
                public const int MAX = 4;
            }


            public static class Input
            {
                public const float MIN_DRAG_DISTANCE = 0.25f;
            }

            public static class Shooter
            {
                public const float LOADED_SUIKA_ROTATION_SPEED = 100f;

                public const float DEFAULT_AUTOFIRE_TIME = 7.5f;
                public const float AUTOFIRE_COUNTDOWN_START = 3f;

                public const double AUTOFIRE_MIN_TIME_LIMIT = 1f; // 설정 가능한 최소 시간
                public const double AUTOFIRE_TIME_BONUS = 0.5f; // 추가 시간 보정 (설정한 시간보다 길도록)

                public const float FIRE_COOLDOWN_TIME = 0.25f; // 최소 발사 간격
                
                public const float BASE_FIRE_POWER = 20f;
                public const float MIN_FIRE_POWER_RATIO = 0.05f;
                public const float MAX_FIRE_POWER_RATIO = 1f;

                public static float ClampFirePowerRatio(float powerRatio)
                {
                    return Mathf.Clamp(powerRatio, MIN_FIRE_POWER_RATIO, MAX_FIRE_POWER_RATIO);
                }
            }
        }
    }
}