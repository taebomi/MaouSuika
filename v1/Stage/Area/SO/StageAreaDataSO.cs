using SOSG.Stage.Area;
using UnityEngine;

namespace SOSG.Stage.Map
{
    [CreateAssetMenu(menuName = "TaeBoMi/Stage/Area/Stage Area Data", fileName = "StageAreaDataSO")]
    public class StageAreaDataSO : ScriptableObject
    {
        public StageAreaData data;

#if UNITY_EDITOR
        [TextArea, SerializeField] private string memo;
#endif

        public AreaData GetAreaData(int curScore) => data.GetAreaData(curScore);
    }
}