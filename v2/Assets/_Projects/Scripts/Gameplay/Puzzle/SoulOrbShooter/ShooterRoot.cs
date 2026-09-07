using System;
using MaouSuika.Core;
using R3;
using UnityEngine;

namespace MaouSuika.Gameplay
{
    // Shooter Root는 외부에 Shooter view에 쓰이는 거 노출 안하기 위해서야 !!! 쓸데없는 리팩토링 하지마 !!!
    public class ShooterRoot : MonoBehaviour
    {
        [SerializeField] private SoulOrbShooterSettingsSO settings;

        [SerializeField] private SoulOrbShooter shooter;

        [SerializeField] private SoulOrbShooterAimView aimView;
        [SerializeField] private SoulOrbShooterLoadedOrbView loadedOrbView;

        private ShooterInputResolver _inputResolver;
        private SoulOrbShooterViewModel _viewModel;

        public Observable<Unit> Fired => shooter.Fired;
        public bool IsOverflowing => shooter.IsOverflowing;

        public void Initialize(ISoulOrbSpawner soulOrbSpawner, GameplayPlayerInput playerInput)
        {
            shooter.Initialize(soulOrbSpawner, settings);

            _inputResolver = new ShooterInputResolver(playerInput.CurrentShooterSettings);
            playerInput.ShooterSettingsChanged
                .Subscribe(ApplyInputSettings).AddTo(this);

            _viewModel = new SoulOrbShooterViewModel(shooter);
            aimView.Initialize(_viewModel, settings);
            loadedOrbView.Initialize(_viewModel, settings);
        }

        private void ApplyInputSettings(ShooterControlSchemeSettings inputSettings)
        {
            _inputResolver.ApplySettings(inputSettings);
        }

        public void Setup(Func<int> getNextTier)
        {
            shooter.Setup(getNextTier);
            aimView.Setup();
        }

        public void Tick(in ShooterInputSnapshot input, float deltaTime)
        {
            var resolvedInput = _inputResolver.Resolve(in input, deltaTime);
            shooter.Tick(in resolvedInput, deltaTime);
            _viewModel.Tick();
            loadedOrbView.Tick(deltaTime);
        }

        private void OnDestroy()
        {
            _viewModel?.Dispose();
        }

        public void ResetInput()
        {
            _inputResolver.Reset();
            shooter.ClearAim();
        }

        public bool TryDiscardLoadedOrb(out SoulOrbSnapshot snapshot)
        {
            var discarded = shooter.TryDiscardLoadedOrb(out snapshot);
            if (discarded) _inputResolver.Reset();

            return discarded;
        }

        public void Stop()
        {
            _inputResolver.Reset();
            shooter.Stop();
            loadedOrbView.Stop();
        }
    }
}