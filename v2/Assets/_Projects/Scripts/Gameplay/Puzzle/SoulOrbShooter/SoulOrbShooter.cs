using System;
using R3;
using UnityEngine;

namespace MaouSuika.Gameplay
{
    public class SoulOrbShooter : MonoBehaviour
    {
        private SoulOrbShooterSettingsSO _settings;
        private ISoulOrbSpawner _soulOrbSpawner;
        private Func<int> _dequeueNextTier;


        private ContactFilter2D _muzzleContactFilter;
        private readonly Collider2D[] _tempBuffer = new Collider2D[3];

        private SoulOrb _lastShotOrb;
        private float _fireCooldown;

        private readonly Subject<Unit> _fired = new();
        private readonly Subject<SoulOrb> _reloaded = new();
        private readonly ReactiveProperty<float> _autoFireProgress = new(0); // todo: AutoFire 구현
        private readonly ReactiveProperty<AimData?> _aim = new(null);

        public Observable<Unit> Fired => _fired;
        public Observable<SoulOrb> Reloaded => _reloaded;
        public ReadOnlyReactiveProperty<float> AutoFireProgress => _autoFireProgress;
        public ReadOnlyReactiveProperty<AimData?> Aim => _aim;

        public SoulOrb LoadedSoulOrb { get; private set; }
        public MuzzleStatus MuzzleStatus { get; private set; }

        public bool IsFireCooldown => _fireCooldown > 0f;
        public bool CanFire => LoadedSoulOrb && !IsFireCooldown && MuzzleStatus is MuzzleStatus.Clear;
        public bool IsOverflowing => MuzzleStatus is MuzzleStatus.BlockedByLanded;

        private void Awake()
        {
            _muzzleContactFilter = new ContactFilter2D
            {
                useLayerMask = true,
                layerMask = LayerMask.GetMask("SoulOrb"),
                useTriggers = false,
            };
        }

        public void Initialize(ISoulOrbSpawner soulOrbSpawner, SoulOrbShooterSettingsSO settings)
        {
            _settings = settings;
            _soulOrbSpawner = soulOrbSpawner;
        }

        public void Setup(Func<int> dequeueNextTier)
        {
            _fireCooldown = 0f;
            _lastShotOrb = null;
            MuzzleStatus = MuzzleStatus.Clear;

            _dequeueNextTier = dequeueNextTier;
            ReloadSoulOrb();
        }


        public void Tick(in ResolvedShooterInput input, float deltaTime)
        {
            UpdateCooldown(deltaTime);
            MuzzleStatus = ProbeMuzzle();
            HandleInput(input);
        }

        public bool TryDiscardLoadedOrb(out SoulOrbSnapshot snapshot)
        {
            if (!LoadedSoulOrb)
            {
                snapshot = default;
                return false;
            }

            snapshot = new SoulOrbSnapshot(LoadedSoulOrb);

            ClearAim();
            SetCooldown();
            ReloadSoulOrb();

            return true;
        }

        public void Stop()
        {
            LoadedSoulOrb = null;
            _lastShotOrb = null;
            ClearAim();
        }

        public void Fire(AimData aim)
        {
            if (!CanFire) return;

            if (_lastShotOrb) _lastShotOrb.EndActiveShot();

            _lastShotOrb = LoadedSoulOrb;
            LoadedSoulOrb = null;
            _lastShotOrb.Shoot(aim.Direction * _settings.Fire.ShootPower * aim.PowerRatio, aim.PowerRatio);

            SetCooldown();
            ReloadSoulOrb();

            _fired.OnNext(Unit.Default);
        }

        private void ReloadSoulOrb()
        {
            if (LoadedSoulOrb) _soulOrbSpawner.Despawn(LoadedSoulOrb);

            var tier = _dequeueNextTier();
            LoadedSoulOrb = _soulOrbSpawner.Spawn(tier, transform.position, false, SoulOrbSpawnMode.Loaded);

            _reloaded.OnNext(LoadedSoulOrb);
        }

        private void HandleInput(in ResolvedShooterInput input)
        {
            if (input is { FireRequested: true, Aim: { } aim })
            {
                Fire(aim);
                _aim.Value = null;
                return;
            }

            _aim.Value = input.Aim;
        }

        public void ClearAim()
        {
            _aim.Value = null;
        }

        private void SetCooldown()
        {
            _fireCooldown = _settings.Fire.FireCooldown;
        }

        private void UpdateCooldown(float deltaTime)
        {
            if (IsFireCooldown) _fireCooldown -= deltaTime;
        }


        private MuzzleStatus ProbeMuzzle()
        {
            if (!LoadedSoulOrb) return MuzzleStatus.Clear;

            var count = Physics2D.OverlapCircle
                (transform.position, LoadedSoulOrb.Radius, _muzzleContactFilter, _tempBuffer);

            var status = MuzzleStatus.Clear;
            for (var i = 0; i < count; i++)
            {
                if (!_tempBuffer[i].TryGetComponent(out SoulOrb orb)) continue;
                if (orb.HasLanded) return MuzzleStatus.BlockedByLanded;
                status = MuzzleStatus.Occupied;
            }

            return status;
        }
    }
}