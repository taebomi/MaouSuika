using UnityEngine;

namespace SOSG.Stage.Area
{
    [CreateAssetMenu(menuName = "TaeBoMi/Stage/Area/Stage Area Data SO", fileName = "StageAreaDataSO")]
    public class StageAreaDataSO : ScriptableObject
    {
        public StageAreaData data;

#if UNITY_EDITOR
        [TextArea, SerializeField] private string memo;
#endif

        public SOSG.Area.AreaData GetAreaData(int score) => data.GetAreaData(score);
    }
}