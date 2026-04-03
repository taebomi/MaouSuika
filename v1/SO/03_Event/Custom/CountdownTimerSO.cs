using System;
using UnityEngine;

namespace SOSG.Utility.Countdown
{
    [CreateAssetMenu(menuName = "TaeBoMi/Custom Event/Countdown Timer", fileName = "CountdownTimerSO")]
    public class CountdownTimerSO : ScriptableObject
    {
        public event Action<CountdownState> OnStateChanged;
        public CountdownState State { get; private set; }

#if UNITY_EDITOR
        [TextArea, SerializeField] private string memo;
        [SerializeField] private bool debugMode;
#endif

        public void Initialize()
        {
            State = CountdownState.Canceled;
        }

        public void ChangeState(CountdownState state)
        {
            if (State == state)
            {
                return;
            }

#if UNITY_EDITOR
            if (debugMode)
            {
                Debug.Log($"{GetType()} - {State} -> {state} 상태 변경.");
            }
#endif
            State = state;
            OnStateChanged?.Invoke(state);
        }
    }

    public enum CountdownState
    {
        Canceled,
        Started,
        Finished,
    }
}