using System;

namespace SOSG.Stage.Area
{
    [Serializable]
    public class StageAreaData
    {
        public SOSG.Area.AreaData[] areaData;
        public int[] cutOffScore;

        public SOSG.Area.AreaData GetAreaData(int curScore)
        {
            for (var i = 0; i < cutOffScore.Length; i++)
            {
                if (curScore < cutOffScore[i])
                {
                    return areaData[i];
                }
            }

            return areaData[^1];
        }
    }
}