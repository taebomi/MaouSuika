using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MaouSuika.Gameplay
{
    public class PuzzleRegionPresentation : MonoBehaviour
    {
        [SerializeField] private float fadeDuration = 0.3f;
        [SerializeField] private Ease fadeEase = Ease.InOutSine;

        private Tilemap[] _tilemaps;

        private float _alpha = 1f;

        private void Awake()
        {
            _tilemaps = GetComponentsInChildren<Tilemap>();
        }

        public void SetAlpha(float alpha)
        {
            _alpha = alpha;

            foreach (var tilemap in _tilemaps)
            {
                var color = tilemap.color;
                color.a = alpha;
                tilemap.color = color;
            }
        }

        public UniTask FadeToAsync(float targetAlpha, CancellationToken token)
        {
            return DOTween.To(() => _alpha, SetAlpha, targetAlpha, fadeDuration)
                .SetEase(fadeEase)
                .Play()
                .ToUniTask(tweenCancelBehaviour: TweenCancelBehaviour.KillAndCancelAwait, cancellationToken: token);
        }
    }
}