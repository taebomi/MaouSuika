using DG.Tweening;
using System.Collections;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Battle
{
    public class BattleRegion : MonoBehaviour
    {
        [SerializeField] private BattleRegionBackground background;
        [SerializeField] private Transform far;
        [SerializeField] private Transform near;

        public void Prepare(SlideOptions options)
        {
            background.Prepare();

            near.localPosition = new Vector3(0f, options.offset, -10f);
            far.localPosition = new Vector3(0f, options.offset, -10);
        }

        public void Finish()
        {
            near.localPosition = Vector3.zero;
            far.localPosition = Vector3.zero;
        }

        public IEnumerator TransitionOutRoutine(RegionTransitionSO transition)
        {
            var slideRoutine = StartCoroutine(SlideOutRoutine(transition.SlideOptions));
            var fadeRoutine = StartCoroutine(background.FadeOutRoutine(transition.FadeOptions));

            yield return slideRoutine;
            yield return fadeRoutine;
        }

        public IEnumerator TransitionInRoutine(RegionTransitionSO transition)
        {
            var slideRoutine = StartCoroutine(SlideInRoutine(transition.SlideOptions));
            var fadeRoutine = StartCoroutine(background.FadeInRoutine(transition.FadeOptions));

            yield return slideRoutine;
            yield return fadeRoutine;
        }

        public IEnumerator FadeInRoutine(FadeOptions options)
        {
            yield return background.FadeInRoutine(options);
        }

        public IEnumerator FadeOutRoutine(FadeOptions options)
        {
            yield return background.FadeOutRoutine(options);
        }

        public IEnumerator SlideInRoutine(SlideOptions options)
        {
            yield return DOTween.Sequence()
                .Insert(0f,
                    near.DOLocalMoveY(0f, options.duration)
                        .From(options.offset).SetEase(options.slideInEase))
                .Insert(options.interval,
                    far.DOLocalMoveY(0f, options.duration)
                        .From(options.offset).SetEase(options.slideInEase))
                .Play()
                .WaitForCompletion();
        }

        public IEnumerator SlideOutRoutine(SlideOptions options)
        {
            yield return DOTween.Sequence()
                .Insert(0f,
                    near.DOLocalMoveY(options.offset, options.duration)
                        .From(0f).SetEase(options.slideOutEase))
                .Insert(options.interval,
                    far.DOLocalMoveY(options.offset, options.duration)
                        .From(0f).SetEase(options.slideOutEase))
                .Play()
                .WaitForCompletion();
        }
    }
}