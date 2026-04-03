using System;
using UnityEngine;

[CreateAssetMenu(menuName = "TaeBoMi/Built-in Event/Void", fileName = "VoidEventSO", order = 3000)]
public class VoidEventSO : ScriptableObject
{
    public Action OnEventRaised;

#if UNITY_EDITOR
    [TextArea, SerializeField] private string memo;
#endif

    public void RaiseEvent()
    {
        OnEventRaised?.Invoke();
    }
}