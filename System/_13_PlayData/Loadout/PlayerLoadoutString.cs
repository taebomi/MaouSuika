using System;

namespace SOSG.System.PlayData
{
    /// <summary>
    /// 세이브 시 몬스터의 id를 string으로 저장하기 위해 사용함.
    /// </summary>
    [ES3Serializable]
    [Serializable]
    public class PlayerLoadoutString
    {
        public MonsterLoadoutString monsterLoadoutString;
        public string skillString = SkillName.DefaultSkillName;

        public static PlayerLoadoutString CreateDefaultInstance()
        {
            var loadoutString = new PlayerLoadoutString
            {
                monsterLoadoutString = MonsterLoadoutString.CreateDefaultInstance()
            };

            return loadoutString;
        }

        public static PlayerLoadoutString ConvertFrom(PlayerLoadout loadout)
        {
            var loadoutString = new PlayerLoadoutString
            {
                monsterLoadoutString = MonsterLoadoutString.ConvertFrom(loadout.MonsterLoadout)
            };

            return loadoutString;
        }
    }
}