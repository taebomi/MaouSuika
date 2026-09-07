using System;
using DG.Tweening;
using Febucci.UI;
using R3;
using TMPro;
using UnityEngine;

namespace MaouSuika.Gameplay
{
    public partial class ComboIndicatorView : MonoBehaviour
    {
        [SerializeField] private ComboStyleConfigSO styleConfig;

        [SerializeField] private RectTransform container;
        [SerializeField] private ParticleSystem fireParticleSystem;
        [SerializeField] private TextAnimator_TMP valueTextAnimator;
        [SerializeField] private TextAnimator_TMP labelTextAnimator;

        [Header("Show / Hide")]
        [SerializeField] private float slideDistance = 120f;
        [SerializeField] private float transitionDuration = 0.3f;

        [Header("Color Flash")]
        [SerializeField] private float colorPunchDuration = 0.5f;

        private CanvasGroup _canvasGroup;

        private Vector2 _shownPos;
        private bool _shown;

        private Tween _transition;
        private Tween _colorFlashTween;

        private IDisposable _comboChangedSubscription;


        private void Awake()
        {
            _shown = false;

            _canvasGroup = container.GetComponent<CanvasGroup>();

            _shownPos = container.anchoredPosition;

            labelTextAnimator.SetText("<comboPulse a=0.3>combo</comboPulse>");
            container.gameObject.SetActive(false);
        }

        public void Setup(ReadOnlyReactiveProperty<int> currentCombo)
        {
            _comboChangedSubscription?.Dispose();
            _comboChangedSubscription = currentCombo.Subscribe(OnComboChanged);
        }

        private void OnComboChanged(int combo)
        {
            var grade = ComboGrades.GradeOf(combo);
            if (grade is ComboGrade.None)
            {
                if (_shown) PlayHide();
                return;
            }

            var style = styleConfig.IndicatorStyleFor(grade);

            UpdateText(combo);
            ApplyStyle(style);

            if (!_shown) PlayShow();
        }

        private void PlayShow()
        {
            _shown = true;
            _transition?.Kill();

            container.gameObject.SetActive(true);
            container.anchoredPosition = _shownPos + Vector2.right * slideDistance;
            _canvasGroup.alpha = 0f;

            _transition = DOTween.Sequence()
                .Join(container.DOAnchorPos(_shownPos, transitionDuration).SetEase(Ease.OutCubic))
                .Join(_canvasGroup.DOFade(1f, transitionDuration).SetEase(Ease.InOutSine))
                .SetLink(container.gameObject)
                .Play();
        }

        private void PlayHide()
        {
            _shown = false;
            _transition?.Kill();
            _transition = DOTween.Sequence()
                .Join(container
                    .DOAnchorPos(
                        _shownPos + Vector2.right * slideDistance, transitionDuration)
                    .SetEase(Ease.InCubic))
                .Join(_canvasGroup
                    .DOFade(0f, transitionDuration)
                    .SetEase(Ease.InOutSine))
                .SetLink(container.gameObject)
                .OnComplete(() => container.gameObject.SetActive(false))
                .Play();
        }

        private void UpdateText(int combo)
        {
            var value = FormattableString.Invariant(
                $"{{size a=-0.65}}<comboPulse>{combo}</comboPulse>{{/size}}");
            valueTextAnimator.SetText(value, hideText: true);
            valueTextAnimator.SetVisibilityEntireText(isVisible: true, canPlayEffects: true);
            valueTextAnimator.time.RestartTime();
            labelTextAnimator.time.RestartTime();
        }

        private void ApplyStyle(ComboIndicatorStyle style)
        {
            var color = style.Color;

            labelTextAnimator.TMProComponent.color = GetComboLabelColor(color);
            PlayColorFlash(color);
            ApplyParticleStyle(style);
        }

        private void PlayColorFlash(Color targetColor)
        {
            _colorFlashTween?.Kill(true);
            _colorFlashTween = valueTextAnimator.TMProComponent
                .DOColor(targetColor, colorPunchDuration)
                .From(Color.white)
                .SetEase(Ease.OutCubic)
                .SetLink(container.gameObject)
                .Play();
        }

        private void ApplyParticleStyle(ComboIndicatorStyle style)
        {
            var main = fireParticleSystem.main;
            main.startSize = style.StartSize;

            var emission = fireParticleSystem.emission;
            emission.rateOverTime = style.EmissionRate;

            var colorOverLifetime = fireParticleSystem.colorOverLifetime;
            colorOverLifetime.color = style.ColorOverLifetime;
        }

        private Color GetComboLabelColor(Color comboColor)
        {
            const float softenAmount = 0.35f;

            var color = Color.Lerp(comboColor, Color.white, softenAmount);
            return color;
        }
    }
}
