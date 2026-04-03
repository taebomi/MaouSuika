using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Battle
{
    [CreateAssetMenu(fileName = "Hero Visual Config SO", menuName = "Maou Suika/Battle/Hero/Visual Config")]
    public class HeroVisualConfigSO : ScriptableObject
    {
        [field: SerializeField] public float DamageFlashDuration { get; private set; } = 0.25f;
        [field: SerializeField] public HeroDieAnimConfig DieAnimConfig { get; private set; }

        [Header("Victory")]
        [field: SerializeField] public float VictoryJumpHeight { get; private set; } = 1.5f;
        [field: SerializeField] public float VictoryJumpDuration { get; private set; } = 0.3f;
    }
}