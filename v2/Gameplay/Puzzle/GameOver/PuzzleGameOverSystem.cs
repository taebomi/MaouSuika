using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    public partial class PuzzleGameOverSystem : MonoBehaviour
    {
        [SerializeField] private PuzzleGameOverSystemConfigSO config;

        // Event Channel
        [SerializeField] private DangerLevelEventChannelSO dangerLevelEventChannel;
        [SerializeField] private GameOverEventChannelSO gameOverEventChannel;

        [SerializeField] private WarningSensor warningSensor;
        private CriticalSensor _criticalSensor;
        private GameOverTimer _gameOverTimer;

        [SerializeField] private DangerOverlayVisualizer dangerOverlayVisualizer;

        private int _playerIndex;
        private DangerLevel _currentDangerLevel;

        public bool IsGameOver { get; private set; }

        private void Awake()
        {
            _gameOverTimer = new GameOverTimer(config.GameOverTimerDuration);
        }

        public void Initialize(int playerIndex, ShooterSystem shooterSystem)
        {
            _playerIndex = playerIndex;
            warningSensor.Initialize(config.warningSetThreshold, config.warningUnsetThreshold);
            _criticalSensor = new CriticalSensor(shooterSystem, config.CriticalSetThreshold);
        }

        public void ResetSystem()
        {
            _currentDangerLevel = DangerLevel.Safe;

            warningSensor.ResetSensor();
            _criticalSensor.ResetSensor();
            _gameOverTimer.Reset();

            dangerOverlayVisualizer.Setup();
        }

        public void Deactivate()
        {
            dangerOverlayVisualizer.Stop();
        }


        public void Tick(float deltaTime)
        {
            UpdateSensors(deltaTime);
            UpdateDangerLevel();
            ProcessGameOverTimer(deltaTime);
        }

        private void UpdateSensors(float deltaTime)
        {
            warningSensor.Tick(deltaTime);
            _criticalSensor.Tick(deltaTime);
        }

        private DangerLevel EvaluateDangerLevel()
        {
            if (_criticalSensor.IsCritical) return DangerLevel.Critical;
            if (warningSensor.IsDanger) return DangerLevel.Warning;
            return DangerLevel.Safe;
        }

        private void UpdateDangerLevel()
        {
            var nextLevel = EvaluateDangerLevel();
            if (_currentDangerLevel == nextLevel) return;
            _currentDangerLevel = nextLevel;

            if (_currentDangerLevel is DangerLevel.Critical)
            {
                _gameOverTimer.Reset();
            }

            dangerOverlayVisualizer.HandleDangerLevel(_currentDangerLevel);
            dangerLevelEventChannel.RaiseEvent(_playerIndex, _currentDangerLevel);
        }

        private void ProcessGameOverTimer(float deltaTime)
        {
            if (_currentDangerLevel is not DangerLevel.Critical) return;

            var isGameOver = _gameOverTimer.Tick(deltaTime);
            if (isGameOver) GameOver();
        }

        private void GameOver()
        {
            IsGameOver = true;
            gameOverEventChannel.RaiseEvent(_playerIndex);
        }


    }
}