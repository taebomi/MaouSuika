using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Battle
{
    [CreateAssetMenu(fileName = "HeroDataSO", menuName = "Maou Suika/Battle/Hero/Data")]
    public class HeroDataSO : ScriptableObject
    {
        [field: SerializeField] public HeroStats Stats { get; private set; }
    }
}