using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TBM.Extensions;
using UnityEngine;

namespace TBM.Core
{
    public class CountdownTimer
    {
        public event Action<float> Tick;
        public event Action Completed;

        private float _duration;
        private bool _isPaused;

        private CancellationTokenSource _timerCts;
        private readonly CancellationToken _destroyCt;

        public CountdownTimer(CancellationToken destroyCt)
        {
            _destroyCt = destroyCt;
            _duration = 0f;
        }

        public CountdownTimer SetDuration(float duration)
        {
            _duration = duration;
            return this;
        }

        /// <summary>
        /// 설정해둔 duration으로 countdown 실행.
        /// duration 설정 필수(기본값 0).
        /// </summary>
        public void Start()
        {
            Start(_duration);
        }

        /// <summary>
        /// 임시 duration으로 카운트다운 실행.
        /// </summary>
        /// <param name="duration"></param>
        public void Start(float duration)
        {
            Stop();
            _timerCts = CancellationTokenSource.CreateLinkedTokenSource(_destroyCt);
            CountdownAsync(duration, _timerCts.Token).Forget();
        }

        public void Resume()
        {
            _isPaused = false;
        }

        public void Pause()
        {
            _isPaused = true;
        }

        public void Stop()
        {
            _timerCts?.CancelAndDispose();
            _timerCts = null;
            _isPaused = false;
        }

        private async UniTask CountdownAsync(float duration, CancellationToken ct)
        {
            if (duration <= 0f)
            {
                Tick?.Invoke(0f);
                Completed?.Invoke();
                return;
            }

            var remained = duration;
            Tick?.Invoke(remained);

            while (true)
            {
                await UniTask.Yield(ct);
                if (_isPaused) continue;

                remained -= Time.deltaTime;

                if (remained > 0f)
                {
                    Tick?.Invoke(remained);
                    continue;
                }

                Tick?.Invoke(0f);
                Completed?.Invoke();
                return;
            }
        }
    }
}