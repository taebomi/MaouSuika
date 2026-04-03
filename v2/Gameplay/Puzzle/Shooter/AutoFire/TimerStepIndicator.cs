using DG.Tweening;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    public class TimerStepIndicator : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer sr;

        private Sequence _motionSequence;

        private void OnDisable()
        {
            KillAllTweens();
        }

        public void SetVisible(bool value)
        {
            gameObject.SetActive(value);
        }

        public void SetLocalPosition(Vector3 position)
        {
            transform.localPosition = position;
        }

        public void Appear(float duration)
        {
            SetVisible(true);
            _motionSequence?.Kill();
            _motionSequence = DOTween.Sequence()
                .Append(transform.DOScale(1f, duration).From(0f).SetEase(Ease.OutBack))
                .SetLink(gameObject)
                .Play();
        }

        public void Pulse(float pulseScale, float duration)
        {
            if (_motionSequence != null && _motionSequence.IsActive()) return;

            _motionSequence = DOTween.Sequence()
                .Append(transform.DOScale(pulseScale, duration * 0.4f).SetEase(Ease.OutQuad))
                .Append(transform.DOScale(1f, duration * 0.6f).SetEase(Ease.InQuad))
                .SetLink(gameObject)
                .Play();
        }

        public void Disappear(float popScale, float duration)
        {
            _motionSequence?.Kill();

            _motionSequence = DOTween.Sequence()
                .Append(transform.DOScale(popScale, duration * 0.4f).SetEase(Ease.OutQuad))
                .Append(transform.DOScale(0f, duration * 0.6f).SetEase(Ease.InQuad))
                .SetLink(gameObject)
                .OnComplete(() => SetVisible(false))
                .Play();
        }

        // ================================================
        // Color
        // ================================================
        public void SetColor(Color color)
        {
            sr.color = color;
        }


        private void KillAllTweens()
        {
            _motionSequence?.Kill();
        }
    }
}