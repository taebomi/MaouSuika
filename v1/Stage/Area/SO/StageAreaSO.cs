using System;
using UnityEngine;

namespace SOSG.Stage.Area
{
    [CreateAssetMenu(menuName = "TaeBoMi/Stage/Area/Stage Area SO", fileName = "StageAreaSO")]
    public class StageAreaSO : ScriptableObject
    {
        public AreaData CurAreaData { get; private set; } // 현재 밟고 있는 Area Data
        
        public event Func<AreaData> FuncOnGetNextStageAreaData;
        public event Action<AreaData> ActionOnAreaChanged;
        
        public AreaData GetNextAreaData()
        {
            return FuncOnGetNextStageAreaData?.Invoke();
        }

        public void InitializeCurAreaData(AreaData areaData)
        {
            CurAreaData = areaData;
        }

        public void SetCurAreaData(AreaData areaData)
        {
            CurAreaData = areaData;
            ActionOnAreaChanged?.Invoke(areaData);
        }
    }
}