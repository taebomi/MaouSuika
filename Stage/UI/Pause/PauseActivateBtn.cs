using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace SOSG.Stage.UI
{
    public class PauseActivateBtn : MonoBehaviour
    {
        [SerializeField] private RectTransform rt;
        [SerializeField] private Button btn;

        private CancellationTokenSource _destroyCts;

        private void Awake()
        {
            _destroyCts = new CancellationTokenSource();
            btn.interactable = false;
        }

        private void OnDestroy()
        {
            _destroyCts.CancelAndDispose();
        }
    
        public async UniTaskVoid Show()
        {
            await rt.DOAnchorPosX(0f, 0.25f).SetEase(Ease.OutSine).SetUpdate(true).Play().WithCancellation(_destroyCts.Token);
            btn.interactable = true;
        }

        public async UniTaskVoid Hide()
        {
            btn.interactable = false;
            await rt.DOAnchorPosX(rt.sizeDelta.x, 0.25f).SetEase(Ease.InSine).SetUpdate(true).Play().WithCancellation(_destroyCts.Token);
        }
    }
}