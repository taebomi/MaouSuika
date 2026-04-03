using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace SOSG.UI
{
    public class HorizontalScrollAnimator : MonoBehaviour
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private RectTransform scrollRt;

        [SerializeField] private float minWidth = 60f;
        [SerializeField] private float width = 600f;
        [SerializeField] private float duration = 0.35f;
        [SerializeField] private float waitInterval = 0.15f;
        [SerializeField] private bool closedOnAwake;

        private float _height;
        private float _animationTime;

        private bool _isInitialized;

        private enum State
        {
            Closed,
            Animating,
            Opened,
        }

        private State _state;

        public bool IsClosed => _state is State.Closed;
        public bool IsOpened => _state is State.Opened;

        public bool IsAnimating => _state is State.Animating;

        private void Awake()
        {
            _height = scrollRt.sizeDelta.y;
            if (closedOnAwake)
            {
                scrollRt.sizeDelta = new Vector2(minWidth, _height);
                _animationTime = 0f;
                _state = State.Closed;
            }
            else
            {
                scrollRt.sizeDelta = new Vector2(width, _height);
                _animationTime = 1f;
                _state = State.Opened;
            }
        }

        public void Open()
        {
            _state = State.Opened;
            scrollRt.sizeDelta = new Vector2(width, _height);
            _animationTime = duration;
        }

        public async UniTask OpenAsync(CancellationToken ct)
        {
            _state = State.Animating;
            var remainedDuration = duration - _animationTime;
            var timer = 0f;
            var startWidth = scrollRt.sizeDelta.x;
            while (_animationTime < duration)
            {
                var ease = Easing.OutBack(timer, remainedDuration);
                var curWidth = (width - startWidth) * ease;
                scrollRt.sizeDelta = new Vector2(startWidth + curWidth, _height);
                timer += Time.deltaTime;
                _animationTime += Time.deltaTime;
                await UniTask.Yield(ct);
            }

            _state = State.Opened;
            scrollRt.sizeDelta = new Vector2(width, _height);
            _animationTime = duration;
        }

        public async UniTask CloseAsync(CancellationToken ct)
        {
            _state = State.Animating;
            var timer = _animationTime;
            var remainedDuration = _animationTime;
            var startWidth = scrollRt.sizeDelta.x - minWidth;
            
            while (_animationTime > 0f)
            {
                var ease = Easing.InSine(timer, remainedDuration);
                var curWidth = startWidth * ease;
                scrollRt.sizeDelta = new Vector2(curWidth + minWidth, _height);
                _animationTime -= Time.deltaTime;
                timer -= Time.deltaTime;
                await UniTask.Yield(ct);
            }

            _state = State.Closed;
            scrollRt.sizeDelta = new Vector2(minWidth, _height);
            _animationTime = 0f;
        }

        public async UniTask DelayAsync(CancellationToken ct)
        {
            var oriState = _state;
            _state = State.Animating;
            await UniTask.Delay(TimeSpan.FromSeconds(waitInterval), cancellationToken: ct);
            _state = oriState;
        }
    }
}