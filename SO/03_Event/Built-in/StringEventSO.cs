using System;
using UnityEngine;

namespace SOSG.System
{
    [CreateAssetMenu(menuName = "TaeBoMi/Built-in Event/String", fileName = "StringEventSO", order = 3000)]
    public class StringEventSO : ScriptableObject
    {
        public Action<string> OnEventRaised;
    
#if UNITY_EDITOR
        [SerializeField] private bool debugMode;
        [TextArea, SerializeField] private string memo;
#endif
    
        public void RaiseEvent(string value)
        {
#if UNITY_EDITOR
            if (debugMode)
            {
                Debug.Log($"{GetType()} - {value} 이벤트 발생.");
            }
#endif
            OnEventRaised?.Invoke(value);
        }
    }
}