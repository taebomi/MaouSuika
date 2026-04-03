using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using NotImplementedException = System.NotImplementedException;

namespace SOSG.System.Scene
{
    public class AllTransitioner : TransitionerBase
    {
        [SerializeField] private RawImage image;

        public override bool WillHideConversation => true;

        private Sequence _showSequence;
        private Sequence _hideSequence;

        private void Awake()
        {
            _showSequence = DOTween.Sequence().SetAutoKill(false).SetUpdate(true)
                .Append(image.rectTransform.DOScaleY(1f, 0.5f).From(0f).SetEase(Ease.InQuad))
                .Join(image.DOFade(1f, 0.35f).From(0.5f).SetEase(Ease.InQuad));
            _hideSequence = DOTween.Sequence().SetAutoKill(false).SetUpdate(true)
                .Append(image.rectTransform.DOScaleY(0f, 0.5f).From(1f).SetEase(Ease.InQuad))
                .Insert(0.15f, image.DOFade(0.5f, 0.35f).From(1f).SetEase(Ease.InQuad));
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