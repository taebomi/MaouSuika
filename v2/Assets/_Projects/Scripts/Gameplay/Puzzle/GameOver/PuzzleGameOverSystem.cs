using System;
using R3;

namespace MaouSuika.Gameplay
{
    public class PuzzleGameOverSystem : IDisposable
    {
        [Serializable]
        public class Settings
        {
            public float warningEnterTime = 1.5f;
            public float warningExitTime = 1.5f;
            public float countdownEnterTime = 1f;
            public float countdownDuration = 5f;
        }

        private readonly Settings _settings;

        private bool _isWarning;
        private float _warningFlipTimer;

        private float _countdownEnterTimer;
        private float _countdownRemaining;

        private bool IsCountdown => _countdownEnterTimer >= _settings.countdownEnterTime;

        private readonly ReactiveProperty<GameOverPhase> _phase = new();
        public ReadOnlyReactiveProperty<GameOverPhase> Phase => _phase;
        public Observable<Unit> GameOver { get; }

        public PuzzleGameOverSystem(Settings settings)
        {
            _settings = settings;
            GameOver = _phase.Where(phase => phase is GameOverPhase.GameOver).AsUnitObservable();
        }

        public void Dispose()
        {
            _phase?.Dispose();
        }

        public void Setup()
        {
            _phase.Value = GameOverPhase.None;
            _isWarning = false;
            _warningFlipTimer = 0f;
            _countdownEnterTimer = 0f;
            _countdownRemaining = _settings.countdownDuration;
        }

        public void Tick(bool warningZoneOccupied, bool shooterBlocked, float deltaTime)
        {
            if (_phase.Value is GameOverPhase.GameOver) return;

            TickWarning(warningZoneOccupied, deltaTime);
            TickCountdown(shooterBlocked, deltaTime);

            var newPhase = EvaluatePhase();
            if (_phase.Value == newPhase) return;
            _phase.Value = newPhase;
        }

        private void TickWarning(bool warningZoneOccupied, float deltaTime)
        {
            var flipSignal = _isWarning ? !warningZoneOccupied : warningZoneOccupied;
            var flipTime = _isWarning ? _settings.warningExitTime : _settings.warningEnterTime;

            if (!flipSignal)
            {
                _warningFlipTimer = 0f;
                return;
            }

            _warningFlipTimer += deltaTime;
            if (_warningFlipTimer < flipTime) return;

            _isWarning = !_isWarning;
            _warningFlipTimer = 0f;
        }

        private void TickCountdown(bool shooterBlocked, float deltaTime)
        {
            _countdownEnterTimer = shooterBlocked ? _countdownEnterTimer + deltaTime : 0f;

            _countdownRemaining =
                IsCountdown ? _countdownRemaining - deltaTime : _settings.countdownDuration;
        }

        private GameOverPhase EvaluatePhase()
        {
            if (_countdownRemaining <= 0f) return GameOverPhase.GameOver;
            if (IsCountdown) return GameOverPhase.Countdown;
            if (_isWarning) return GameOverPhase.Warning;
            return GameOverPhase.None;
        }
    }
}