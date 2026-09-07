using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

namespace MaouSuika.Gameplay
{
    public class PuzzleGameOverRoot : MonoBehaviour
    {
        [SerializeField] private PuzzleGameOverSystem.Settings settings;

        [SerializeField] private WarningZone warningZone;

        [SerializeField] private GameOverVignetteView vignetteView;

        private PuzzleGameOverSystem _system;

        public ReadOnlyReactiveProperty<GameOverPhase> Phase => _system.Phase;
        public Observable<Unit> GameOver => _system.GameOver;

        public void Initialize()
        {
            _system = new PuzzleGameOverSystem(settings);

            vignetteView.Initialize();
            _system.Phase.Subscribe(ApplyPhase).AddTo(this);
        }

        public void Setup()
        {
            _system.Setup();
            warningZone.Setup();
        }

        public void Tick(bool shooterBlocked, float deltaTime)
        {
            warningZone.Tick(deltaTime);
            _system.Tick(warningZone.IsOccupied, shooterBlocked, deltaTime);
        }

        private void ApplyPhase(GameOverPhase phase)
        {
            vignetteView.SetPhase(phase);
        }
    }
}