using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


namespace MaouSuika.Gameplay
{
    [RequireComponent(typeof(CanvasGroup))]
    public class GameOverResultView : MonoBehaviour
    {
        [SerializeField] private Button restartBtn;

        private CanvasGroup _canvasGroup;

        private Tween _fadeTween;

        public void Initialize(Action onRestartBtnClicked)
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            restartBtn.onClick.AddListener(new UnityAction(onRestartBtnClicked));
        }

        public void SetVisible(bool visible, bool instant = false)
        {
            _fadeTween?.Kill();
            if (instant)
            {
                _canvasGroup.alpha = visible ? 1 : 0;
                gameObject.SetActive(visible);
                return;
            }

            if (visible) gameObject.SetActive(true);

            const float fullDuration = 0.15f;
            const float speed = 1 / fullDuration;
            var endAlpha = visible ? 1 : 0;
            _fadeTween = DOTween.To(() => _canvasGroup.alpha, x => _canvasGroup.alpha = x, endAlpha, speed)
                .SetEase(Ease.OutSine)
                .SetSpeedBased(true)
                .SetLink(gameObject)
                .OnComplete(() =>
                {
                    if (!visible) gameObject.SetActive(false);
                })
                .Play();
        }
    }
}