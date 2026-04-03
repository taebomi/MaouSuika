using FMODUnity;
using UnityEngine;

[CreateAssetMenu(fileName = "LineSfxData", menuName = "TaeBoMi/Custom Data/Dialogue/Line Sfx")]
public class LineSfxDataSO : ScriptableObject
{
    public EventReference low;
    public EventReference normal;
    public EventReference high;
}