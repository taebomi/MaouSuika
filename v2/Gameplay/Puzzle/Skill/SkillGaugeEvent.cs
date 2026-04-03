namespace TBM.MaouSuika.Gameplay.Puzzle
{
    public readonly struct SkillGaugeEvent
    {
        public readonly float NormalizedGauge;

        public SkillGaugeEvent(float normalizedGauge)
        {
            NormalizedGauge = normalizedGauge;
        }
    }
}
