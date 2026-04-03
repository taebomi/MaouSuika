using TBM.MaouSuika.Core;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    public class ShooterFireCooldownTimer
    {
        public bool IsCooldown => _cooldownTimer > 0f;
        private float _cooldownTimer;

        public void Reset()
        {
            _cooldownTimer = GameRule.Puzzle.Shooter.FIRE_COOLDOWN_TIME;
        }

        public void Tick(float deltaTime)
        {
            if (_cooldownTimer <= 0f) return;

            _cooldownTimer -= deltaTime;
        }
    }
}