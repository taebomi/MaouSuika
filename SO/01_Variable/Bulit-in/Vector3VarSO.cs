using UnityEngine;

[CreateAssetMenu(menuName = "TaeBoMi/Built-in Var/Vector3", fileName = "Vector3VarSO", order = 1100)]
public class Vector3VarSO : ScriptableObject
{
    [field:SerializeField] public Vector3 Value { get; private set; }
    
    public void Set(Vector3 value)
    {
        Value = value;
    }
}
