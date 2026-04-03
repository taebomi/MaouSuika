using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace SOSG.System.Scene
{
    public class LoadingFader : MonoBehaviour
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private RawImage fadingImage;
        
        public void SetSortingOrder(int value)
        {
            canvas.sortingOrder = value;
        }

        public async UniTask ShowAsync(CancellationToken token)
        {
            gameObject.SetActive(true);
            await fadingImage.DOFade(0f, 1f).From(1f).SetEase(Ease.InOutQuad).Play()
                .AwaitForComplete(TweenCancelBehaviour.KillAndCancelAwait, token);
        }

        public async UniTask HideAsync(CancellationToken token)
        {
            gameObject.SetActive(true);
            await fadingImage.DOFade(1f, 1f).From(0f).SetEase(Ease.InOutQuad).Play()
                .AwaitForComplete(TweenCancelBehaviour.KillAndCancelAwait, token);
            gameObject.SetActive(false);
        }
    }
}