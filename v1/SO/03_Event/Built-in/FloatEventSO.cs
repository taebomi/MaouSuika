using System;
using UnityEngine;

[CreateAssetMenu(menuName = "TaeBoMi/Built-in Event/Float", fileName = "FloatEventSO", order = 3000)]
public class FloatEventSO : ScriptableObject
{
    public Action<float> OnEventRaised;
    
    public void RaiseEvent(float value)
    {
        OnEventRaised?.Invoke(value);
    }
}
