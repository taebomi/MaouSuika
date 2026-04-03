using System;
using UnityEngine;

[CreateAssetMenu(menuName = "TaeBoMi/Built-in Event/Int", fileName = "IntEventSO", order = 3000)]
public class IntEventSO : ScriptableObject
{
    public Action<int> OnEventRaised;

#if UNITY_EDITOR
    [TextArea, SerializeField] private string memo;
#endif
    public void RaiseEvent(int value)
    {
        OnEventRaised?.Invoke(value);
    }
}
