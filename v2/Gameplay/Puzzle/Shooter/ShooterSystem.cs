using System;
using System.Collections;
using TBM.MaouSuika.Core;
using TBM.MaouSuika.Core.Input;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    public class ShooterSystem : MonoBehaviour
    {
        // # Modules
        [SerializeField] private ShooterLoaderModule loaderModule;
        [SerializeField] private ShooterAimModule aimModule;
        [SerializeField] private ShooterAutoFireModule autoFireModule;

        private ShooterInputModule _inputModule;
        private ShooterBlockSensor _blockSensor;
        private ShooterFireCooldownTimer _fireCooldownTimer;

        // # Fields
        private ShooterState _prevState;
        private ShooterState _currentState;

        public bool IsBlocked => _blockSensor.IsBlocked;
        private bool IsLoaded => loaderModule.IsLoaded;

        public event Action Fired;

        private void Awake()
        {
            _blockSensor = new ShooterBlockSensor();
            _fireCooldownTimer = new ShooterFireCooldownTimer();
        }

        public void Initialize(SuikaSystem suikaSystem, SuikaQueueSystem queueSystem, InputController inputController,
            PuzzleArea area)
        {
            autoFireModule.Initialize();
            loaderModule.Initialize(suikaSystem, queueSystem);
            _inputModule = new ShooterInputModule(inputController, area);
        }

        private void OnDestroy()
        {
            if (_inputModule != null)
            {
                _inputModule.Dispose();
                _inputModule = null;
            }
        }

        // ################################################
        // Game State
        // ################################################
        public void Setup()
        {
            _currentState = ShooterState.None;

            _fireCooldownTimer.Reset();

            _inputModule.Setup();
            aimModule.Setup();
            loaderModule.Clear();
            loaderModule.LoadNext();
        }

        public void HandleGameStarted()
        {
        }

        public void HandleGameOver()
        {
        }

        public void Tick(ShooterInputResult inputResult, float deltaTime)
        {
            if (!IsLoaded) return;

            ProcessInput(inputResult, deltaTime);
            
            _fireCooldownTimer.Tick(deltaTime);
            _blockSensor.UpdateBlockState(transform.position, loaderModule.LoadedRadius);
            
            RefreshState();
            ExecuteActions(deltaTime);
        }

        private void ProcessInput(ShooterInputResult inputResult, float deltaTime)
        {
            var command = _inputModule.Process(inputResult, deltaTime);
            aimModule.HandleCommand(command, _currentState);

            if (command.Type is ShooterInputCommand.CommandType.Fire) TryFire(command.AimData);
        }


        private void TryFire(AimData aimData)
        {
            if (_currentState is not ShooterState.None) return;

            if (!IsLoaded) return;

            var suika = loaderModule.ExtractForFire();
            loaderModule.LoadNext();

            _fireCooldownTimer.Reset();
            autoFireModule.Stop();
            RefreshState();


            var velocity = aimData.Direction * (aimData.PowerRatio * GameRule.Puzzle.Shooter.BASE_FIRE_POWER);
            suika.Shoot(velocity);

            Fired?.Invoke();
        }

        // ################
        // PRIVATE
        // ################

        private void HandleStateChanged(ShooterState prevState, ShooterState newState)
        {
            if (prevState == newState) return;

            loaderModule.UpdateVisual(newState);

            var isCooldownFinished = (prevState & ShooterState.Cooldown) != 0 &&
                                     (newState & ShooterState.Cooldown) == 0;
            if (isCooldownFinished)
            {
                autoFireModule.Setup(loaderModule.LoadedRadius);
            }
        }

        private void ExecuteActions(float deltaTime)
        {
            if (_currentState is not ShooterState.None) return;

            loaderModule.Tick(deltaTime, _currentState);
            autoFireModule.Tick(deltaTime);

            if (autoFireModule.IsCompleted) TryFire(aimModule.AutoFireAim);
        }

        private void RefreshState()
        {
            _prevState = _currentState;
            _currentState = CalculateCurrentState();
            HandleStateChanged(_prevState, _currentState);
        }

        private ShooterState CalculateCurrentState()
        {
            var state = ShooterState.None;

            if (_blockSensor.IsBlocked) state |= ShooterState.Blocked;
            if (_fireCooldownTimer.IsCooldown) state |= ShooterState.Cooldown;

            return state;
        }
    }
}