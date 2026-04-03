using UnityEngine;

[CreateAssetMenu(menuName = "TaeBoMi/Built-in Var/Float", fileName = "FloatVarSO", order = 1100)]
public class FloatVarSO : ScriptableObject
{
    [field: SerializeField] public float Value { get; private set; }

    public void Set(float value)
    {
        Value = value;
    }
}