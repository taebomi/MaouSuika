using DG.Tweening;
using MaouSuika.Core;
using TMPro;
using UnityEngine;

namespace MaouSuika.Gameplay
{
    // 3. High — Slam: 크게 등장해서 쾅 박히고, 잠시 버틴 뒤 올라감
    [CreateAssetMenu(fileName = "ComboPopupSlamMotion", menuName = MenuPath.Gameplay.COMBO + "Motion/Slam")]
    public class ComboPopupSlamMotionSO : ComboPopupMotionSO
    {
        [SerializeField] private float startScaleMultiplier = 1.8f;
        [SerializeField] private float slamDuration = 0.12f;
        [SerializeField] private float impactPunch = 0.15f;
        [SerializeField] private float riseDuration = 0.5f;
        [SerializeField] private float riseDistance = 0.6f;

        public override Sequence CreateSequence(Transform target, TMP_Text tmp, float holdTime)
        {
            var baseScale = target.localScale;
            target.localScale = baseScale * startScaleMultiplier;

            return DOTween.Sequence()
                .Append(target.DOScale(baseScale, slamDuration).SetEase(Ease.InCubic))
                .Append(target.DOPunchScale(Vector3.one * impactPunch, 0.15f, vibrato: 8))
                .AppendInterval(holdTime)
                .Append(target.DOMoveY(riseDistance, riseDuration).SetRelative().SetEase(Ease.OutCubic))
                .Join(tmp.DOFade(0f, riseDuration));
        }
    }
}