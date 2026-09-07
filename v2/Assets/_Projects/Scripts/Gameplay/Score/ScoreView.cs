using System;
using DG.Tweening;
using R3;
using TMPro;
using UnityEngine;

namespace MaouSuika.Gameplay
{
    public partial class ScoreView : MonoBehaviour
    {
        [SerializeField] private ScoreDigitReel[] reels;
        [SerializeField] private Material digitMaterialPreset;

        [Header("Spin")]
        [SerializeField] private AnimationCurve spinDurationBySteps;
        [SerializeField] private float maxRevolutions = 3f;

        [Header("Settle")]
        [SerializeField] private float settleDuration = 0.12f;
        [SerializeField] private float minOvershoot = 0.2f;
        [SerializeField] private float maxOvershoot = 1f;

        [Header("Impact")]
        [SerializeField] private Color impactColor = Color.whiteSmoke;
        [SerializeField] private float impactDuration = 0.3f;
        [SerializeField] private float impactScale = 1.25f;

        private Material _digitMaterial;

        private float[] _reelValue;
        private float[] _lerpStart;
        private float[] _bounceTarget;
        private float[] _finalTarget;

        private Color _originalColor;
        private float _originalSize;

        private Tween _tween;

        private IDisposable _scoreChangedSubscription;

        public void Initialize()
        {
            _digitMaterial = new Material(digitMaterialPreset);
            _originalColor = _digitMaterial.GetColor(ShaderUtilities.ID_FaceColor);
            _originalSize = reels[0].transform.localScale.x;

            foreach (var reel in reels)
            {
                var tmpArr = reel.GetComponentsInChildren<TMP_Text>();
                foreach (var tmp in tmpArr) tmp.fontSharedMaterial = _digitMaterial;
            }

            _reelValue = new float[reels.Length];
            _lerpStart = new float[reels.Length];
            _bounceTarget = new float[reels.Length];
            _finalTarget = new float[reels.Length];
        }

        public void Setup(Observable<int> totalScoreAdded, int curScore)
        {
            _scoreChangedSubscription?.Dispose();

            SetScoreImmediately(curScore);
            _scoreChangedSubscription = totalScoreAdded.Subscribe(UpdateScore);
        }

        private void OnDestroy()
        {
            if (_digitMaterial) Destroy(_digitMaterial);
            _scoreChangedSubscription?.Dispose();
        }

        private void UpdateScore(int newScore)
        {
            _tween?.Kill();

            _digitMaterial.SetColor(ShaderUtilities.ID_FaceColor, _originalColor);

            ApplySize(_originalSize);

            var divisor = 1f;
            var maxSpinCount = maxRevolutions * 10f;
            var maxClampedDiff = 0f;

            for (var i = 0; i < reels.Length; i++)
            {
                var newCount = Mathf.Floor(newScore / divisor);
                var rawDiff = newCount - _reelValue[i];
                var clampedDiff = Mathf.Min(rawDiff, maxSpinCount);
                maxClampedDiff = Mathf.Max(maxClampedDiff, clampedDiff);

                _finalTarget[i] = newCount;
                _lerpStart[i] = newCount - clampedDiff;

                var bounce = clampedDiff > 0f
                    ? Mathf.Lerp(minOvershoot, maxOvershoot, clampedDiff / maxSpinCount)
                    : 0f;
                _bounceTarget[i] = newCount + bounce;

                divisor *= 10f;
            }

            var duration = spinDurationBySteps.Evaluate(maxClampedDiff);

            _tween = DOTween.Sequence()
                // Reel 회전
                .Append(DOTween
                    .To(() => 0f, ApplyMain, 1f, duration)
                    .SetEase(Ease.OutQuad))

                // 최종 숫자로 착지
                .Append(DOTween
                    .To(() => 0f, ApplyBounce, 1f, settleDuration)
                    .SetEase(Ease.OutQuad))

                // 착지 순간 즉시 Flash + 확대
                .AppendCallback(() =>
                {
                    _digitMaterial.SetColor(ShaderUtilities.ID_FaceColor, impactColor);
                    ApplySize(impactScale);
                })

                // 원래 색상과 크기로 복귀
                .Append(_digitMaterial
                    .DOColor(_originalColor, ShaderUtilities.ID_FaceColor, impactDuration)
                    .SetEase(Ease.OutQuad))
                .Join(DOTween
                    .To(
                        () => reels[0].transform.localScale.x,
                        ApplySize,
                        _originalSize,
                        impactDuration)
                    .SetEase(Ease.OutBack))
                .SetLink(gameObject)
                .Play();
        }

        private void SetScoreImmediately(int score)
        {
            _tween?.Kill();

            _digitMaterial.SetColor(ShaderUtilities.ID_FaceColor, _originalColor);
            ApplySize(_originalSize);
            var divisor = 1f;

            for (var i = 0; i < reels.Length; i++)
            {
                var reelValue = Mathf.Floor(score / divisor);
                SetReelValue(i, reelValue);

                divisor *= 10f;
            }
        }


        private void ApplyMain(float p)
        {
            ApplyReels(_lerpStart, _bounceTarget, p);
        }

        private void ApplyBounce(float p)
        {
            ApplyReels(_bounceTarget, _finalTarget, p);
        }

        private void ApplyReels(float[] from, float[] to, float p)
        {
            for (var i = 0; i < reels.Length; i++)
            {
                var value = Mathf.Lerp(from[i], to[i], p);
                SetReelValue(i, value);
            }
        }

        private void SetReelValue(int index, float value)
        {
            _reelValue[index] = value;
            reels[index].SetDigit(value % 10f);
        }

        private void ApplySize(float size)
        {
            foreach (var reel in reels)
            {
                reel.SetSize(size);
            }
        }
    }
}