using System.Collections;
using DG.Tweening;
using TBM.Extensions;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    public class PuzzleRegion : MonoBehaviour
    {
        private SpriteRenderer[] _spriteRenderers;
        private Tilemap[] _tilemaps;

        private void Awake()
        {
            _spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
            _tilemaps = GetComponentsInChildren<Tilemap>();
        }

        public void Prepare() => ApplyAlpha(0f);

        public IEnumerator FadeInRoutine(PuzzleRegionTransitionSO transition)
        {
            yield return DOTween.To(() => 0f, ApplyAlpha, 1f, transition.Duration)
                .SetEase(transition.Ease).Play().WaitForCompletion();
        }

        public IEnumerator FadeOutRoutine(PuzzleRegionTransitionSO transition)
        {
            yield return DOTween.To(() => 1f, ApplyAlpha, 0f, transition.Duration)
                .SetEase(transition.Ease).Play().WaitForCompletion();
        }

        private void ApplyAlpha(float alpha)
        {
            foreach (var sr in _spriteRenderers)
                sr.color = sr.color.WithAlpha(alpha);
            foreach (var tm in _tilemaps)
                tm.color = tm.color.WithAlpha(alpha);
        }
    }
}
