using UnityEngine;

[CreateAssetMenu(menuName = "TaeBoMi/Built-in Data/Float Array", fileName = "FloatArrDataSO", order = 2000)]
public class FloatArrDataSO : ScriptableObject
{
    [field: SerializeField] public float[] Data { get; private set; }
}