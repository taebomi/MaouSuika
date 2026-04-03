using TBM.MaouSuika.Core.Settings;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    public interface IShooterInputStrategy
    {
        ShooterInputType InputType { get; }

        void Enter();
        void Exit();
        
        ShooterInputCommand Process(ShooterInputResult input, float deltaTime);
        void UpdateConfig(InputProfile profile, string scheme);
    }
}