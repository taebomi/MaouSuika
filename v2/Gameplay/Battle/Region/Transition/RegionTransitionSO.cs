using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Battle
{
    [CreateAssetMenu(fileName = "RegionTransition", menuName = "Maou Suika/Battle/Region/Transition")]
    public class RegionTransitionSO : ScriptableObject
    {
        [field: SerializeField] public SlideOptions SlideOptions { get; private set; }
        [field: SerializeField] public FadeOptions FadeOptions { get; private set; }
        [field: SerializeField] public float SlideDelay { get; private set; } = 0.25f;
    }
}