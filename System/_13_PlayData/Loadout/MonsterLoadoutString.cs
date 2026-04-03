using System;

namespace SOSG.System.PlayData
{
    [Serializable]
    [ES3Serializable]
    public class MonsterLoadoutString
    {
        public static string[] DefaultMonsterLoadoutString =
        {
            MonsterName.Seoul, MonsterName.Blueming, MonsterName.Gob, MonsterName.Skeleton, // Common
            MonsterName.Gnoll, MonsterName.LizardNyam, MonsterName.Mino, // Uncommon
            MonsterName.LivingArmor, MonsterName.Arachne, MonsterName.Lich, // Rare
            MonsterName.Dragon // Epic
        };
        
        public string[] monsterIdArr = new string[MonsterLoadout.Size];


        public string this[int tier]
        {
            get => monsterIdArr[tier];
            set => monsterIdArr[tier] = value;
        }

        public static MonsterLoadoutString CreateDefaultInstance()
        {
            var loadoutString = new MonsterLoadoutString();
            for (var i = 0; i < DefaultMonsterLoadoutString.Length; i++)
            {
                loadoutString[i] = DefaultMonsterLoadoutString[i];
            }

            return loadoutString;
        }

        public static MonsterLoadoutString ConvertFrom(MonsterLoadout monsterLoadout)
        {
            var loadoutString = new MonsterLoadoutString();
            for (var i = 0; i < MonsterLoadout.Size; i++)
            {
                loadoutString[i] = monsterLoadout[i].id;
            }

            return loadoutString;
        }
    }
}