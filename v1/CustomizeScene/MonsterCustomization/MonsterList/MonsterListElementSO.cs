using UnityEngine;

namespace SOSG.Customization.Monster
{
    [CreateAssetMenu(fileName = "Monster List Element SO", menuName = "TaeBoMi/Customization/Monster List Element")]
    public class MonsterListElementSO : ScriptableObject
    {
        [field: SerializeField] public Sprite[] OutlineSpriteArr { get; private set; }
        [field: SerializeField] public Sprite[] TierSpriteArr { get; private set; }
    }
}