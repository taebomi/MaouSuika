using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SOSG.MainScene
{
    public class Title : MonoBehaviour
    {
        [SerializeField] private Animator ani;

        [SerializeField] private RectTransform shineRt;

        private CancellationTokenSource _enableCts;

        private readonly int _impactHash = Animator.StringToHash("Impact");

        private void OnEnable()
        {
            _enableCts?.Dispose();
            _enableCts = new CancellationTokenSource();
        }

        private void OnDisable()
        {
            _enableCts.Cancel();
        }

        public void SetActive(bool value)
        {
            gameObject.SetActive(value);
        }

        public async UniTaskVoid PlayImpact()
        {
            while (_enableCts.Token.IsCancellationRequested is false)
            {
                ani.Play(_impactHash);
                await UniTask.Delay(TimeSpan.FromSeconds(Random.Range(3f, 5f)),
                    cancellationToken: _enableCts.Token);
            }
        }

        public async UniTaskVoid PlayShine()
        {
            while (_enableCts.Token.IsCancellationRequested is false)
            {
                await shineRt
                    .DOAnchorPosX(500f, 1f)
                    .From(new Vector2(-500f, 0f))
                    .SetEase(Ease.InOutSine).WithCancellation(_enableCts.Token);
                await UniTask.Delay(TimeSpan.FromSeconds(Random.Range(2.5f, 5f)),
                    cancellationToken: _enableCts.Token);
            }
        }
    }
}