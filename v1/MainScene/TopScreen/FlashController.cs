using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace SOSG.MainScene
{
    public class FlashController : MonoBehaviour
    {
        [SerializeField] private IntEventSO bgmTimingEventSO;

        [SerializeField] private CanvasGroup canvasGroup;

        private const float FadeInTime = 0.15f;
        private const float FadeOutTime = 1f;

        private CancellationTokenSource _destroyCts;

        private void Awake()
        {
            canvasGroup.blocksRaycasts = false;
            canvasGroup.alpha = 0f;
            bgmTimingEventSO.OnEventRaised += OnBgmTimingEventRaised;
            _destroyCts = new CancellationTokenSource();
        }

        private void OnDestroy()
        {
            _destroyCts.CancelAndDispose();
            bgmTimingEventSO.OnEventRaised -= OnBgmTimingEventRaised;
        }

        private void OnBgmTimingEventRaised(int timing)
        {
            if (timing == 5)
            {
                Fade().Forget();
            }
        }

        private async UniTaskVoid Fade()
        {
            canvasGroup.blocksRaycasts = true;
            await canvasGroup.DOFade(1f, FadeInTime).From(0f).SetUpdate(true).Play().WithCancellation(_destroyCts.Token);
            bgmTimingEventSO.RaiseEvent(10);
            await canvasGroup.DOFade(0f, FadeOutTime).From(1f).SetUpdate(true).Play().WithCancellation(_destroyCts.Token);
            canvasGroup.blocksRaycasts = false;
        }
    }
}