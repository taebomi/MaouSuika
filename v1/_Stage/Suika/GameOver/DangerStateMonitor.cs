using System;
using SOSG.Stage.GameOver;
using UnityEngine;
using UnityEngine.Serialization;

namespace SOSG.Stage.Suika.GameOver
{
    public class DangerStateMonitor : MonoBehaviour
    {
        [SerializeField] private WarningArea warningArea;
        [SerializeField] private GameOverCountdownTimer gameOverCountdownTimer;

        public DangerState DangerState { get; private set; }
        public event Action DangerStateChanged;
        
        private void OnEnable()
        {
            warningArea.WarningSet += CheckState;
            warningArea.WarningUnset += CheckState;
            gameOverCountdownTimer.CountdownStarted += CheckState;
            gameOverCountdownTimer.CountdownCanceled += CheckState;
        }

        private void OnDisable()
        {
            warningArea.WarningSet -= CheckState;
            warningArea.WarningUnset -= CheckState;
            gameOverCountdownTimer.CountdownStarted -= CheckState;
            gameOverCountdownTimer.CountdownCanceled -= CheckState;
        }

        private void CheckState()
        {
            if (gameOverCountdownTimer.IsCountdown)
            {
                ChangeState(DangerState.Countdown);
                return;
            }

            if (warningArea.IsWarning)
            {
                ChangeState(DangerState.Warning);
                return;
            }

            ChangeState(DangerState.None);
        }

        private void ChangeState(DangerState state)
        {
            if (DangerState == state)
            {
                return;
            }

            DangerState = state;
            DangerStateChanged?.Invoke();
        }
    }
}