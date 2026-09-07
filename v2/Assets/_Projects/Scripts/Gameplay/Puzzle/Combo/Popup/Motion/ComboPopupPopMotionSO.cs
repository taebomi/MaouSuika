using DG.Tweening;
using MaouSuika.Core;
using TMPro;
using UnityEngine;

namespace MaouSuika.Gameplay
{
    // 2. Mid — Pop: 튀어나오며 오버슈트, 통통한 느낌
    [CreateAssetMenu(fileName = "ComboPopupPopMotion", menuName = MenuPath.Gameplay.COMBO + "Motion/Pop")]
    public class ComboPopupPopMotionSO : ComboPopupMotionSO
    {
        [SerializeField] private float popDuration = 0.25f;
        [SerializeField] private float overshoot = 2.5f;
        [SerializeField] private float riseDuration = 0.6f;
        [SerializeField] private float riseDistance = 0.7f;
        [SerializeField] private float fadeOutTime = 0.25f;

        public override Sequence CreateSequence(Transform target, TMP_Text tmp, float holdTime)
        {
            var baseScale = target.localScale;
            target.localScale = Vector3.zero;

            return DOTween.Sequence()
                .Append(target.DOScale(baseScale, popDuration).SetEase(Ease.OutBack, overshoot))
                .AppendInterval(holdTime)
                .Append(target.DOMoveY(riseDistance, riseDuration).SetRelative().SetEase(Ease.OutCubic))
                .Insert(popDuration + holdTime + riseDuration - fadeOutTime, tmp.DOFade(0f, fadeOutTime));
        }
    }
}
