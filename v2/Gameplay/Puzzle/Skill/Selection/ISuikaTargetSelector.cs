namespace TBM.MaouSuika.Gameplay.Puzzle
{
    public interface ISuikaTargetSelector
    {
        SuikaObject HoveredSuika { get; }
        bool IsConfirmed { get; }
        bool IsCancelled { get; }
        void Tick(float deltaTime);
        void Dispose();
    }
}
