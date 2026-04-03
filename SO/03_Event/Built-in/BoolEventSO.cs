using System;
using UnityEngine;

[CreateAssetMenu(menuName = "TaeBoMi/Built-in Event/Bool", fileName = "BoolEventSO", order = 3000)]
public class BoolEventSO : ScriptableObject
{
    public event Action<bool> OnEventRaised;

#if UNITY_EDITOR
    [TextArea, SerializeField] private string memo;
    [SerializeField] private bool debugMode;
#endif

    public void RaiseEvent(bool value)
    {
#if UNITY_EDITOR
        if (debugMode)
        {
            Debug.Log($"{name} - {value} 이벤트 발생");
        }
#endif
        OnEventRaised?.Invoke(value);
    }
}