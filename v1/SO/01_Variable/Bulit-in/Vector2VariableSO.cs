using System;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "TaeBoMi/Built-in Var/Vector2", fileName = "Vector2VarSO",order = 1100)]
public class Vector2VariableSO : ScriptableObject
{
    public Action<Vector2> OnValueChanged;
    [field:FormerlySerializedAs("value")] public Vector2 Value { get; private set; }

    public void Set(Vector2 value)
    {
        Value = value;
        OnValueChanged?.Invoke(value);
    }

    public void Initialize(Vector2 value)
    {
        Value = value;
    }
    
}
