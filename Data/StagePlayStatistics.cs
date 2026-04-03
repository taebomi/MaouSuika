using System;
using UnityEngine.Serialization;

namespace SOSG.System.PlayData
{
    [Serializable]
    public class StagePlayStatistics
    {
        public int score;
        public int shotCount;
        public int[] createdMonsterCount = new int[11];
        public int highCombo;
        public int lastAreaIdx;
    }
}