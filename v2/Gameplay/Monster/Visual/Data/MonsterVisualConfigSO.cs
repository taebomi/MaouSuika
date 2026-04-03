using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Monster
{
    [CreateAssetMenu(fileName = "Monster Visual Config", menuName = "Maou Suika/Monster/Visual Config")]
    public class MonsterVisualConfigSO : ScriptableObject
    {
        [Header("Victory")]
        [field: SerializeField] public float VictoryJumpHeight { get; private set; } = 1.5f;
        [field: SerializeField] public float VictoryJumpDuration { get; private set; } = 0.3f;
    }
}
