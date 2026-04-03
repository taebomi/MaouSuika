using System;
using UnityEngine;

namespace SOSG.Stage.Area
{
    [CreateAssetMenu(menuName = "TaeBoMi/Stage/Area/Stage Area Manager SO", fileName = "StageAreaManagerSO")]
    public class StageAreaManagerSO : ScriptableObject
    {
        public SOSG.Area.AreaData CurAreaData { get; private set; }

        public event Action<SOSG.Area.AreaData> ActionOnAreaChanged;


        public void Initialize(SOSG.Area.AreaData initAreaData)
        {
            CurAreaData = initAreaData;
        }

        public void SetCurAreaData(SOSG.Area.AreaData newAreaData)
        {
            CurAreaData = newAreaData;
            ActionOnAreaChanged?.Invoke(newAreaData);
        }
    }
}