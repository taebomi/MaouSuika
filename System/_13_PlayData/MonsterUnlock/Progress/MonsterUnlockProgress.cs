using System;
using System.Collections.Generic;
using SOSG.System.PlayData;

namespace SOSG.System.MonsterUnlock
{
    [ES3Serializable]
    [Serializable]
    public class MonsterUnlockProgress
    {
        public HashSet<string> UnlockedIdSet = new(MonsterLoadoutString.DefaultMonsterLoadoutString);

        public bool IsUnlocked(string id) => UnlockedIdSet.Contains(id);
        public void Unlock(string id) => UnlockedIdSet.Add(id);
    }
}