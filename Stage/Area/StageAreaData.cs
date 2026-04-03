using System;
using SOSG.Stage.Area;

namespace SOSG.Stage.Map
{
    [Serializable]
    public class StageAreaData
    {
        public AreaData[] areaData;
        public int[] cutOffScore;

        public AreaData GetAreaData(int curScore)
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