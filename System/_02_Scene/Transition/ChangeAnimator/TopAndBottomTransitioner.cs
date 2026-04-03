using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace SOSG.System.Scene
{
    public class TopAndBottomTransitioner : TransitionerBase
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private RectTransform topRt;
        [SerializeField] private RectTransform bottomRt;
        
        public override bool WillHideConversation => false;

        private Sequence _showSequence;
        private Sequence _hideSequence;

        public void Awake()
        {
            _showSequence = DOTween.Sequence().SetAutoKill(false).SetUpdate(true)
                .Append(topRt.DOScaleX(1f, 0.5f).From(0f).SetEase(Ease.InQuad))
                .Join(bottomRt.DOScaleX(1f, 0.5f).From(0f).SetEase(Ease.InQuad))
                .Join(canvasGroup.DOFade(1f, 0.35f).From(0.5f).SetEase(Ease.InQuad));
            _hideSequence = DOTween.Sequence().SetAutoKill(false).SetUpdate(true)
                .Append(topRt.DOScaleX(0f, 0.5f).From(1f).SetEase(Ease.InQuad))
                .Join(bottomRt.DOScaleX(0f, 0.5f).From(1f).SetEase(Ease.InQuad))
                .Insert(0.15f, canvasGroup.DOFade(0.5f, 0.35f).From(1f).SetEase(Ease.InQuad));
        }

        public override async UniTask ShowAsync()
        {
            gameObject.SetActive(true);
            _showSequence.Rewind();
            await _showSequence.Play()
                .AwaitForComplete(TweenCancelBehaviour.KillAndCancelAwait, destroyCancellationToken);
        }

        public override async UniTask HideAsync()
        {
            gameObject.SetActive(true);
            _hideSequence.Rewind();
            await _hideSequence.Play()
                .AwaitForComplete(TweenCancelBehaviour.KillAndCancelAwait, destroyCancellationToken);
            gameObject.SetActive(false);
        }
    }
}