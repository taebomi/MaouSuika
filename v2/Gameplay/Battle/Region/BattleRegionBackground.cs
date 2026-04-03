using System;
using System.Collections;
using DG.Tweening;
using TBM.Extensions;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TBM.MaouSuika.Gameplay.Battle
{
    public class BattleRegionBackground : MonoBehaviour
    {
        private SpriteRenderer[] _spriteRenderers;
        private Tilemap[] _tilemaps;

        private void Awake()
        {
            _spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
            _tilemaps = GetComponentsInChildren<Tilemap>();
        }

        public void Prepare() => ApplyAlpha(0f);

        public IEnumerator FadeInRoutine(FadeOptions options)
        {
            yield return DOTween.To(() => 0f, ApplyAlpha, 1f, options.duration).SetEase(options.ease).Play()
                .WaitForCompletion();
        }

        public IEnumerator FadeOutRoutine(FadeOptions options)
        {
            yield return DOTween.To(() => 1f, ApplyAlpha, 0f, options.duration).SetEase(options.ease).Play()
                .WaitForCompletion();
        }

        private void ApplyAlpha(float alpha)
        {
            foreach (var spriteRenderer in _spriteRenderers)
            {
                spriteRenderer.color = spriteRenderer.color.WithAlpha(alpha);
            }

            foreach (var tilemap in _tilemaps)
            {
                tilemap.color = tilemap.color.WithAlpha(alpha);
            }
        }
    }
}