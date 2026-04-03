using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    public class DangerOverlayVisualizer : MonoBehaviour
    {
        [SerializeField] private DangerFeedbackSettingsSO feedbackSettings;

        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Image vignetteImage;

        private float _currentIntensity;
        private float _targetAlpha;
        private float _targetIntensity;

        private Material _material;

        private Tweener _fadeTween;
        private Tweener _intensityTween;

        private void Awake()
        {
            InitializeVignetteImage();
        }

        private void OnDestroy()
        {
            if (_material != null)
            {
                Destroy(_material);
                _material = null;
            }

            _intensityTween?.Kill();
            _fadeTween?.Kill();
        }

        public void Setup()
        {
            _intensityTween?.Kill();
            _fadeTween?.Kill();

            _currentIntensity = 0f;
            _targetAlpha = 0f;
            _targetIntensity = 0f;
            ApplyMaterialProperties(0f);

            vignetteImage.gameObject.SetActive(false);
            canvasGroup.alpha = 0f;
        }

        public void HandleDangerLevel(DangerLevel level)
        {
            var alpha = feedbackSettings.GetAlpha(level);
            FadeTo(alpha);

            var intensity = feedbackSettings.GetIntensity(level);
            PlayIntensityTween(intensity);
        }

        public void Stop()
        {
            HandleDangerLevel(DangerLevel.Safe);
        }

        private void InitializeVignetteImage()
        {
            if (vignetteImage.material == null) return;

            _material = new Material(vignetteImage.material);
            vignetteImage.material = _material;
        }

        private void ApplyMaterialProperties(float intensity)
        {
            feedbackSettings.ApplyTo(_material, intensity);
        }

        private void FadeTo(float alpha)
        {
            if (Mathf.Approximately(_targetAlpha, alpha)) return;

            _targetAlpha = alpha;
            _fadeTween?.Kill();

            if (alpha > 0f) vignetteImage.gameObject.SetActive(true);

            _fadeTween = canvasGroup
                .DOFade(alpha, feedbackSettings.FadeSpeed)
                .SetSpeedBased(true)
                .OnComplete(() =>
                {
                    _fadeTween = null;
                    if (alpha <= 0f) vignetteImage.gameObject.SetActive(false);
                })
                .Play();
        }

        private void PlayIntensityTween(float targetIntensity)
        {
            if (Mathf.Approximately(_targetIntensity, targetIntensity)) return;

            _targetIntensity = targetIntensity;
            _intensityTween?.Kill();

            _intensityTween = DOTween.To(
                    () => _currentIntensity,
                    intensity =>
                    {
                        _currentIntensity = intensity;
                        ApplyMaterialProperties(intensity);
                    },
                    targetIntensity, feedbackSettings.TransitionDuration)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => _intensityTween = null)
                .Play();
        }
    }
}