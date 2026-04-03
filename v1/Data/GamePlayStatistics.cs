using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace SOSG.System.PlayData
{
    [ES3Serializable]
    [Serializable]
    public class GamePlayStatistics
    {
        public int playCount;

        public int highScore;

        public int shotCount;
        public int[] createdMonsterCount = new int[11];

        public int highCombo;

        public int maxAreaIdx;


        public void Add(StagePlayStatistics other)
        {
            playCount++;
            highScore = Math.Max(highScore, other.score);
            for (var i = 0; i < 11; i++)
            {
                createdMonsterCount[i] += other.createdMonsterCount[i];
            }

            shotCount += other.shotCount;
            highCombo = Math.Max(highCombo, other.highCombo);
            maxAreaIdx = Mathf.Max(maxAreaIdx, other.lastAreaIdx);
        }
    }
}