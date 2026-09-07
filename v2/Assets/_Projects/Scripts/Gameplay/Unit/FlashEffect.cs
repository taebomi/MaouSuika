using DG.Tweening;
using UnityEngine;

namespace MaouSuika.Gameplay
{
    /// <summary>
    /// MPB 단독 점유
    /// </summary>
    public class FlashEffect
    {
        private static readonly int FlashAmountID = Shader.PropertyToID("_FlashAmount");

        private const float DURATION = 0.12f;

        private readonly SpriteRenderer _renderer;
        private readonly MaterialPropertyBlock _mpb;
        private readonly TweenCallback<float> _apply;
        private readonly TweenCallback _reset;

        private Tween _tween;

        public FlashEffect(SpriteRenderer renderer)
        {
            _renderer = renderer;
            _mpb = new MaterialPropertyBlock();
            _reset = Reset;
            _apply = Apply;
        }

        public void Play()
        {
            _tween?.Kill();
            _tween = DOVirtual.Float(1f, 0f, DURATION, _apply)
                .SetEase(Ease.OutQuad)
                .SetLink(_renderer.gameObject)
                .OnComplete(_reset)
                .Play();
        }

        public void Clear()
        {
            _tween?.Kill();
            Reset();
        }

        private void Apply(float value)
        {
            _renderer.GetPropertyBlock(_mpb);
            _mpb.SetFloat(FlashAmountID, value);
            _renderer.SetPropertyBlock(_mpb);
        }

        private void Reset()
        {
            _tween = null;
            _renderer.SetPropertyBlock(null);
        }
    }
}