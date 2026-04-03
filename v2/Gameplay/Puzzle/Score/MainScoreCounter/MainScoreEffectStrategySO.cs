using DG.Tweening;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    public abstract class MainScoreEffectStrategySO : ScriptableObject
    {
        public abstract Sequence CreateSequence(IMainScoreCounter counter, int endScore);
    }
}