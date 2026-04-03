using System;
using UnityEngine;

namespace SOSG.Stage.Area
{
    [CreateAssetMenu(menuName = "TaeBoMi/Stage/Area/AreaChangedEventSO", fileName = "AreaChangedEventSO")]
    public class AreaDataVarSO : ScriptableObject
    {
        public AreaData Value { get; private set; }
        public event Action<AreaData> ActionOnAreaChanged;

#if UNITY_EDITOR
        [SerializeField] private string memo;
#endif

        public void Initialize(AreaData data)
        {
            Value = data;
        }

        public void Set(AreaData newAreaData)
        {
            Value = newAreaData;
            ActionOnAreaChanged?.Invoke(newAreaData);
        }
    }
}