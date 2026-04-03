using TBM.Core;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    public class CriticalSensor
    {
        public bool IsCritical => _debouncer.CurrentState;

        private readonly ShooterSystem _shooterSystem;
        private readonly StateDebouncer _debouncer;

        public CriticalSensor(ShooterSystem shooterSystem, float setThreshold)
        {
            _shooterSystem = shooterSystem;

            _debouncer = new StateDebouncer(setThreshold);
        }

        public void ResetSensor()
        {
            _debouncer.Setup(_shooterSystem.IsBlocked);
        }

        public void Tick(float deltaTime)
        {
            _debouncer.Update(_shooterSystem.IsBlocked, deltaTime);
        }

        public override string ToString()
        {
            var text = "Not Initialized.";
            if (_debouncer == null) return text;

            text += $"State[{_debouncer.CurrentState}] / Time[{_debouncer.Timer}]";
            return text;
        }
    }
}