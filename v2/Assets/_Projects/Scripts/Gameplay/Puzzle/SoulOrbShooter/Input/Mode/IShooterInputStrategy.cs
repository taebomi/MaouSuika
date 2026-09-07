
namespace MaouSuika.Gameplay
{
    public interface IShooterInputStrategy
    {
        void Enter();
        void Exit();

        ResolvedShooterInput Resolve(in ShooterInputSnapshot input, float deltaTime);
    }
}