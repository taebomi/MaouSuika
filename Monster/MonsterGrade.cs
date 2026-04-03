using System.Collections.Generic;

namespace SOSG.Monster
{
    public enum MonsterGrade
    {
        Common = 3,
        Uncommon = 6,
        Rare = 9,
        Epic = 10,
    }

    public static class MonsterGradeCache
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
    }
}