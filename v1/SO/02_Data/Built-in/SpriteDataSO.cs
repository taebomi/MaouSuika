using UnityEngine;

[CreateAssetMenu(menuName = "TaeBoMi/Built-in Data/Sprite", fileName = "SpriteDataSO", order = 2000)]
public class SpriteDataSO : ScriptableObject
{
    [field: SerializeField] public Sprite[] Data { get; private set; }
}