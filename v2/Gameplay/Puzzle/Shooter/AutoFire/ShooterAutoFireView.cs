using System;
using System.Collections.Generic;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    public class ShooterAutoFireView : MonoBehaviour
    {
        [SerializeField] private List<TimerStepIndicator> dotsByPosition;
        [SerializeField] private List<TimerStepIndicator> dotsByOrder;

        private int _activeRange;
        private int _lastActiveCount;

        private AutoFireConfigSO _config;

        public void Initialize(AutoFireConfigSO config)
        {
            _config = config;
        }

        /// <summary>
        /// Reload 시에 Reloaded 된 Suika와 동기화 
        /// </summary>
        public void Setup(float suikaRadius, float totalTime)
        {
            _activeRange = Mathf.Min(dotsByOrder.Count, Mathf.FloorToInt(totalTime));
            _lastActiveCount = -1;
            ArrangeDots(suikaRadius);
            UpdateDotsColor(_activeRange);
        }

        /// <summary>
        /// 해당 Suika radius에 맞게 dot 배열
        /// </summary>
        /// <param name="suikaRadius"></param>
        private void ArrangeDots(float suikaRadius)
        {
            var radius = Mathf.Max(suikaRadius + _config.StepRadiusOffset, _config.StepMinRadius);

            var angleStep = (_config.StepGap / radius) * Mathf.Rad2Deg;
            var totalAngle = angleStep * (dotsByPosition.Count - 1);
            var startAngle = -90f - (totalAngle * 0.5f);

            for (var i = 0; i < dotsByPosition.Count; i++)
            {
                var angleRad = (startAngle + (angleStep * i)) * Mathf.Deg2Rad;

                var x = Mathf.Cos(angleRad) * radius;
                var y = Mathf.Sin(angleRad) * radius;

                dotsByPosition[i].SetLocalPosition(new Vector3(x, y, 0f));
                dotsByPosition[i].SetVisible(false);
            }
        }

        public void UpdateTimerDisplay(float remainingTime)
        {
            if (remainingTime > _activeRange) return;

            var currentActiveCount = Mathf.CeilToInt(remainingTime);

            if (_lastActiveCount == currentActiveCount) return;

            UpdateDotsColor(currentActiveCount);
            // 남은 시간이 이전 시간보다 늘어난 경우 (혹시 모를 상황 처리) 
            if (currentActiveCount > _lastActiveCount)
            {
                for (var i = _lastActiveCount; i < currentActiveCount; i++)
                {
                    if (i < 0 || i >= _activeRange) continue;

                    dotsByOrder[i].Appear(_config.StepAppearDuration);
                }
            }
            else
            {
                for (var i = 0; i < currentActiveCount; i++)
                {
                    if (i >= _activeRange) continue;

                    dotsByOrder[i].Pulse(_config.StepScaleOffset, _config.StepPulseDuration);
                }

                for (var i = currentActiveCount; i < _lastActiveCount; i++)
                {
                    if (i < 0 || i >= _activeRange) continue;

                    dotsByOrder[i].Disappear(_config.StepScaleOffset, _config.StepDisappearDuration);
                }
            }

            _lastActiveCount = currentActiveCount;
        }

        public void Hide()
        {
            foreach (var step in dotsByPosition)
            {
                step.Disappear(_config.StepScaleOffset, _config.StepDisappearDuration);
            }
        }


        private void UpdateDotsColor(int currentCount)
        {
            var color = _config.GetColorByRemainingCount(currentCount);
            for (var i = 0; i < _activeRange; i++)
            {
                dotsByOrder[i].SetColor(color);
            }
        }
    }
}