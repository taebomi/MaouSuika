using System;
using UnityEngine;

[CreateAssetMenu(menuName = "TaeBoMi/Built-in Var/Int", fileName = "IntVarSO",order = 1100)]
public class IntVariableSO : ScriptableObject
{
    public Action<int> OnValueChanged;
    [field:SerializeField] public int Value { get; private set; }

    public void Initialize(int value)
    {
        Value = value;
    }

    public void Set(int value)
    {
        Value = value;
        OnValueChanged?.Invoke(value);
    }
}
