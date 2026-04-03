using System;
using TBM.MaouSuika.Core;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    public class ShooterAutoFireModule : MonoBehaviour
    {
        [SerializeField] private ShooterAutoFireView view;

        [SerializeField] private AutoFireConfigSO config;

        public bool IsCompleted => _timer.IsCompleted;

        private ShooterAutoFireTimer _timer;

        private void Awake()
        {
            _timer = new ShooterAutoFireTimer(config.TimeLimit, config.PreDelay);
        }

        public void Initialize()
        {
            view.Initialize(config);
        }

        public void Setup(float suikaRadius)
        {
            _timer.Reset();
            view.Setup(suikaRadius, _timer.TimeLimit);
        }

        public void Tick(float deltaTime)
        {
            _timer.Tick(deltaTime);
            view.UpdateTimerDisplay(_timer.RemainedTime);
        }

        public void Stop()
        {
            view.Hide();
        }
    }
}