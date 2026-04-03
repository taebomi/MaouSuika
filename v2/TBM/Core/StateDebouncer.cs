using System;

namespace TBM.Core
{
    public class StateDebouncer
    {
        private readonly float _activateThreshold;
        private readonly float _deactivateThreshold;

        public bool CurrentState { get; private set; }
        public float Timer { get; private set; }

        public event Action StateChanged;

        public StateDebouncer(float activateThreshold, float deactivateThreshold = 0f)
        {
            _activateThreshold = activateThreshold;
            _deactivateThreshold = deactivateThreshold;
        }

        public void Setup(bool state)
        {
            CurrentState = state;
            Timer = 0f;
        }

        public void Update(bool rawInput, float deltaTime)
        {
            if (rawInput == CurrentState)
            {
                Timer = 0f;
                return;
            }

            Timer += deltaTime;

            var requiredTime = rawInput ? _activateThreshold : _deactivateThreshold;
            if (Timer < requiredTime)
            {
                return;
            }

            Timer = 0f;
            ChangeState(rawInput);
        }

        private void ChangeState(bool newState)
        {
            CurrentState = newState;
            StateChanged?.Invoke();
        }

        public override string ToString()
        {
            return
                $"State[{CurrentState}]\n" +
                $"Timer[{Timer}]\n" +
                $"Threshold(activate/deactivate)[{_activateThreshold}/{_deactivateThreshold}]";
        }
    }
}