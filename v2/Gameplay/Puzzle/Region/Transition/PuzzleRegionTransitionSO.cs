using DG.Tweening;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    [CreateAssetMenu(fileName = "PuzzleRegionTransition", menuName = "Maou Suika/Puzzle/Region/Transition")]
    public class PuzzleRegionTransitionSO : ScriptableObject
    {
        [field: SerializeField] public float Duration { get; private set; } = 0.5f;
        [field: SerializeField] public Ease Ease { get; private set; } = Ease.Linear;
    }
}
