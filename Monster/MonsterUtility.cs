using System.Collections.Generic;

namespace SOSG.Monster
{
    public static class MonsterUtility
    {
        public static List<MonsterGrade> MonsterGradeList = new()
        {
            MonsterGrade.Common,
            MonsterGrade.Uncommon,
            MonsterGrade.Rare,
            MonsterGrade.Epic,
        };

        public static List<MonsterGrade> CreateMonsterGradeListInstance() => new(MonsterGradeList);

        public static int GetIndex(MonsterGrade grade) => MonsterGradeList.IndexOf(grade);

        public static MonsterGrade GetGrade(int tier)
        {
            return tier switch
            {
                <= (int)MonsterGrade.Common => MonsterGrade.Common,
                <= (int)MonsterGrade.Uncommon => MonsterGrade.Uncommon,
                <= (int)MonsterGrade.Rare => MonsterGrade.Rare,
                _ => MonsterGrade.Epic
            };
        }
    }
}