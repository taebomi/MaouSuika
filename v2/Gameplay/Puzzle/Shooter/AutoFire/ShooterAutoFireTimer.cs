namespace TBM.MaouSuika.Gameplay.Puzzle
{
    public class ShooterAutoFireTimer
    {
        public bool IsCompleted => RemainedTime <= 0f;

        public float TimeLimit { get; private set; }
        public float RemainedTime { get; private set; }

        private readonly float _preDelay;
        private float _remainedPreDelay;

        public ShooterAutoFireTimer(float timerLimit, float preDelay = 0.25f)
        {
            _preDelay = preDelay;
            TimeLimit = timerLimit;
            Reset();
        }

        public void Reset()
        {
            _remainedPreDelay = _preDelay;
            RemainedTime = TimeLimit;
        }

        public void Tick(float deltaTime)
        {
            if (IsCompleted) return;

            if (_remainedPreDelay > 0f)
            {
                _remainedPreDelay -= deltaTime;
                if (_remainedPreDelay <= 0f) _remainedPreDelay = 0f;
                return;
            }

            if (RemainedTime > 0f)
            {
                RemainedTime -= deltaTime;
                if (RemainedTime <= 0f) RemainedTime = 0f;
            }
        }
    }
}