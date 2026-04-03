using TBM.MaouSuika.Core.Settings;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    public class ShooterNoneStrategy : IShooterInputStrategy
    {
        public ShooterInputType InputType => ShooterInputType.None;
        public void Enter() { }
        public void Exit() { }

        public ShooterInputCommand Process(ShooterInputResult input, float deltaTime)
        {
            return ShooterInputCommand.Idle();
        }

        public void UpdateConfig(InputProfile profile, string scheme) { }
    }
}