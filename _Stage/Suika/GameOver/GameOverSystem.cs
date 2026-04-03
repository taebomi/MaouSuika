using System;
using SOSG.Stage.GameOver;
using SOSG.Stage.Player;
using SOSG.Stage.SplitScreenMode;
using UnityEngine;
using UnityEngine.Serialization;

namespace SOSG.Stage.Suika.GameOver
{
    public class GameOverSystem : MonoBehaviour
    {
        [SerializeField] private PlayerManager playerManager;
        [SerializeField] private PlayerGameOverEventSO playerGameOverEventSO;
        
        [field: SerializeField] public DangerStateMonitor DangerStateMonitor { get; private set; }
        [SerializeField] private WarningArea warningArea;
        [SerializeField] private GameOverCountdownTimer countdownTimer;

        private void OnEnable()
        {
            countdownTimer.CountdownFinished += OnCountdownFinished;
        }

        private void OnDisable()
        {
            countdownTimer.CountdownFinished -= OnCountdownFinished;
        }

        public void OnStageStarted()
        {
            warningArea.StartCheck();
        }

        private void OnCountdownFinished()
        {
            playerGameOverEventSO.Invoke(playerManager);
        }
    }
}