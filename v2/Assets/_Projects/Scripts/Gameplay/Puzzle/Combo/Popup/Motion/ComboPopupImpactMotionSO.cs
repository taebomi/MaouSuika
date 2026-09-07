using DG.Tweening;
using MaouSuika.Core;
using TMPro;
using UnityEngine;

namespace MaouSuika.Gameplay
{
    // 4. Extreme — Impact: 슬램 + 회전 진동 + 색 번쩍, 오래 버팀
    [CreateAssetMenu(fileName = "ComboPopupImpactMotion", menuName = MenuPath.Gameplay.COMBO + "Motion/Impact")]
    public class ComboPopupImpactMotionSO : ComboPopupMotionSO
    {
        [SerializeField] private float startScaleMultiplier = 2.4f;
        [SerializeField] private float slamDuration = 0.1f;
        [SerializeField] private float impactPunch = 0.25f;
        [SerializeField] private float shakeRotation = 12f;
        [SerializeField] private Color flashColor = Color.white;
        [SerializeField] private float flashDuration = 0.08f;
        [SerializeField] private float riseDuration = 0.5f;
        [SerializeField] private float riseDistance = 0.5f;

        public override Sequence CreateSequence(Transform target, TMP_Text tmp, float holdTime)
        {
            var baseScale = target.localScale;
            var baseColor = tmp.color;
            target.localScale = baseScale * startScaleMultiplier;
            tmp.color = flashColor;

            return DOTween.Sequence()
                .Append(target.DOScale(baseScale, slamDuration).SetEase(Ease.InExpo))
                .Append(tmp.DOColor(baseColor, flashDuration))
                .Join(target.DOPunchScale(Vector3.one * impactPunch, 0.2f, vibrato: 10))
                .Join(target.DOPunchRotation(new Vector3(0f, 0f, shakeRotation), 0.2f, vibrato: 12))
                .AppendInterval(holdTime)
                .Append(target.DOMoveY(riseDistance, riseDuration).SetRelative().SetEase(Ease.OutCubic))
                .Join(tmp.DOFade(0f, riseDuration));
        }
    }
}
