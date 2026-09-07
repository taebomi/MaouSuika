using System;
using R3;
using UnityEngine;

namespace MaouSuika.Gameplay
{
    public class SoulOrbShooterViewModel : IDisposable
    {
        private readonly SoulOrbShooter _shooter;
        
        private float _reloadTimer;
        
        private readonly ReactiveProperty<ShooterVisualState> _visualState = new();

        public ReadOnlyReactiveProperty<ShooterVisualState> VisualState => _visualState;
        public ReadOnlyReactiveProperty<AimData?> Aim => _shooter.Aim;
        public Observable<SoulOrb> Reloaded => _shooter.Reloaded;
        public SoulOrb LoadedSoulOrb => _shooter.LoadedSoulOrb;

        public SoulOrbShooterViewModel(SoulOrbShooter shooter)
        {
            _shooter = shooter;
        }

        public void Tick()
        {
            _visualState.Value = Evaluate();
        }

        public void Dispose() => _visualState.Dispose();

        private ShooterVisualState Evaluate()
        {
            if (_shooter.IsFireCooldown) return ShooterVisualState.Cooldown;
            if (_shooter.MuzzleStatus is not MuzzleStatus.Clear) return ShooterVisualState.Blocked;
            return ShooterVisualState.Ready;
        }
    }
}