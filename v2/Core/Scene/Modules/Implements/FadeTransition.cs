using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace TBM.MaouSuika.Core.Scene
{
    public class FadeTransition : TransitionBase
    {
        [SerializeField] private CanvasGroup canvasGroup;

        [SerializeField] private float fadeDuration = 0.25f;

        protected override async UniTask PerformShowAsync(CancellationToken token)
        {
            await canvasGroup.DOFade(1f, fadeDuration).From(0f).SetEase(Ease.OutQuad)
                .Play().AwaitForComplete(TweenCancelBehaviour.KillAndCancelAwait, token);
        }

        protected override async UniTask PerformHideAsync(CancellationToken token)
        {
            await canvasGroup.DOFade(0f, fadeDuration).From(1f).SetEase(Ease.OutQuad)
                .Play().AwaitForComplete(TweenCancelBehaviour.KillAndCancelAwait, token);
        }
    }
}