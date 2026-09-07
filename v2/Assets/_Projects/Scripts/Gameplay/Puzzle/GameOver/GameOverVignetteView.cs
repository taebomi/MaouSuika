using System;
using DG.Tweening;
using Febucci.Numbers;
using UnityEngine;
using UnityEngine.UI;
using Tween = DG.Tweening.Tween;

namespace MaouSuika.Gameplay
{
    [RequireComponent(typeof(Image))]
    public class GameOverVignetteView : MonoBehaviour
    {
        [SerializeField] private GameOverVignetteSettingsSO noneSettings;
        [SerializeField] private GameOverVignetteSettingsSO warningSettings;
        [SerializeField] private GameOverVignetteSettingsSO countdownSettings;
        [SerializeField] private float transitionDuration = 0.25f;

        private Image _image;
        private Material _material;
        private VignetteSettings _currentSettings;

        private bool _visible;
        private Tween _tween;

        public void Initialize()
        {
            _image = GetComponent<Image>();
            _material = new Material(_image.material);
            _image.material = _material;
        }

        public void Setup()
        {
            _currentSettings = noneSettings.value;
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            if (_material != null) Destroy(_material);
        }

        public void SetPhase(GameOverPhase phase)
        {
            var fromSettings = _currentSettings;
            var toSettings = GetVignetteSettings(phase);
            _tween?.Kill();
            _tween = DOTween.To(() => 0f,
                    t =>
                    {
                        _currentSettings = VignetteSettings.Lerp(fromSettings, toSettings, t);
                        _currentSettings.ApplyTo(_material);
                    }, 1f,
                    transitionDuration)
                .SetEase(Ease.InOutSine)
                .SetLink(gameObject);
            if (phase is not (GameOverPhase.Countdown or GameOverPhase.Warning))
            {
                _tween.OnComplete(() => gameObject.SetActive(false));
            }

            gameObject.SetActive(true);
            _tween.Play();
        }

        private VignetteSettings GetVignetteSettings(GameOverPhase phase)
        {
            return phase switch
            {
                GameOverPhase.Warning => warningSettings.value,
                GameOverPhase.Countdown => countdownSettings.value,
                _ => noneSettings.value
            };
        }
    }
}