using System;
using UnityEngine;


[CreateAssetMenu(menuName = "TaeBoMi/Built-in Var/Bool", fileName = "BoolVarSO", order = 1100)]
public class BoolVariableSO : ScriptableObject
{
    public event Action<bool> OnValueChanged;
    public bool Value { get; private set; }

#if UNITY_EDITOR
    [TextArea, SerializeField] private string memo;
#endif

    public void Initialize(bool value)
    {
        Value = value;
    }

    public void Set(bool value)
    {
        Value = value;
        OnValueChanged?.Invoke(value);
    }
}