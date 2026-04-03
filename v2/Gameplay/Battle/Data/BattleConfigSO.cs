using AYellowpaper.SerializedCollections;
using TBM.MaouSuika.Gameplay.Unit;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Battle
{
    [CreateAssetMenu(fileName = "BattleConfigSO", menuName = "Maou Suika/Battle/Config")]
    public class BattleConfigSO : ScriptableObject
    {
        [field: SerializeField] public float KnockbackDuration { get; private set; } = 0.25f;
        [SerializeField] private SerializedDictionary<MonsterGrade, float> knockbackDistances;

        public float GetKnockbackDist(MonsterGrade grade) => knockbackDistances[grade];

        [Header("Party")]
        [field: SerializeField] public float HeroSpacing { get; private set; } = 1.5f;
        [field: SerializeField] public float HeroEnterSpeed { get; private set; } = 5f;
        [field: SerializeField] public float MonsterRetreatSpeed { get; private set; } = 3f;

        [Header("Victory")]
        [field: SerializeField] public float VictoryDisplayDuration { get; private set; } = 2f;
    }
}