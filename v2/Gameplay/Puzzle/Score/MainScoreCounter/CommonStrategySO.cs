using DG.Tweening;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    [CreateAssetMenu(fileName = "CommonStrategySO", menuName = "TBM/Score/Main Score Counter/Common Strategy")]
    public class CommonStrategySO : MainScoreEffectStrategySO
    {
        [SerializeField] private float rollingDuration = 0.5f;
        [SerializeField] private Ease rollingEase = Ease.OutExpo;

        [SerializeField] private float punchScale = 0.1f;
        [SerializeField] private float punchDuration = 0.2f;
        [SerializeField] private int punchVibrato = 10;
        [SerializeField] private float punchElasticity = 1f;
        [SerializeField] private Ease punchEase = Ease.OutQuad;

        [SerializeField] private Color flashColor = new Color(1f, 0.8f, 0.2f);
        [SerializeField] private float flashDuration = 0.5f;
        [SerializeField] private Ease flashEase = Ease.InQuad;

        public override Sequence CreateSequence(IMainScoreCounter counter, int endScore)
        {
            var sequence = DOTween.Sequence()
            // Rolling
            .Append(DOTween.To(() => counter.DisplayScore, counter.SetScore, endScore, rollingDuration)
                .SetEase(rollingEase))
            // Punch
            .Join(counter.TargetTr
                .DOPunchScale(Vector3.one * punchScale, punchDuration, punchVibrato, punchElasticity)
                .SetEase(punchEase))
            // Flash
            .Join(counter.TargetTmp.DOColor(counter.BaseColor, flashDuration)
                .From(flashColor).SetEase(flashEase));
            return sequence;
        }
    }
}