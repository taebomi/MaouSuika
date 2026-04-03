using System;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    public class SkillModel
    {
        public float CurrentGauge { get; private set; }
        public float MaxGauge { get; private set; }
        public float NormalizedGauge => MaxGauge > 0f ? CurrentGauge / MaxGauge : 0f;
        public bool IsSkillReady => CurrentGauge >= MaxGauge;

        public event Action<SkillGaugeEvent> GaugeChanged;
        public event Action SkillActivated;

        public void Initialize(float maxGauge)
        {
            MaxGauge = maxGauge;
        }

        public void Setup()
        {
            CurrentGauge = 0f;
        }

        public void AddGauge(float amount)
        {
            if (IsSkillReady) return;

            CurrentGauge = Mathf.Min(CurrentGauge + amount, MaxGauge);
            GaugeChanged?.Invoke(new SkillGaugeEvent(NormalizedGauge));

            if (IsSkillReady)
                SkillActivated?.Invoke();
        }

        public void ConsumeGauge()
        {
            CurrentGauge = 0f;
            GaugeChanged?.Invoke(new SkillGaugeEvent(0f));
        }
    }
}
