using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using MaouSuika.Core;
using TBM.Core;
using UnityEngine;

namespace MaouSuika.Gameplay
{
    /*
     * todo:
     * 2. Active Shot 발사체에만 충돌 연출 적용
     * 효과음, Hit 애니메이션 등 연출
     */
    public class SoulOrb : MonoBehaviour
    {
        [SerializeField] private SoulOrbSettingsSO settings;

        [SerializeField] private SoulOrbShell shell;
        [SerializeField] private SoulOrbMonster monster;

        [SerializeField] private CircleCollider2D orbCollider;

        private Rigidbody2D _rb;

        private SoulOrbPhase _phase;

        private bool _spinStopRequested;
        private bool _isActiveShot;

        private Func<SoulOrb, SoulOrb, bool> _tryMerge;

        private CancellationTokenSource _spawnCts;

        public MonsterDataSO MonsterData { get; private set; }
        public int Tier { get; private set; }
        public int CreationOrder { get; private set; }
        public float Scale { get; private set; }
        public bool HasLanded { get; private set; }

        public Vector2 RbPosition => _rb.position;
        public float Radius => Scale * 0.5f;
        public bool IsMergeable => _phase == SoulOrbPhase.Idle;
        public bool CanBeTargeted => _phase is SoulOrbPhase.Idle;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        public void Initialize(Func<SoulOrb, SoulOrb, bool> tryMerge)
        {
            _tryMerge = tryMerge;
            shell.Initialize();
            monster.Initialize(settings.Monster);
        }


        public void Setup(SoulOrbSetupData setupData)
        {
            _spawnCts.CancelAndDispose();
            _spawnCts = new CancellationTokenSource();

            ResetPhysics();

            transform.localScale = Vector3.one;

            _isActiveShot = false;
            _spinStopRequested = false;

            Tier = setupData.Tier;
            Scale = setupData.Scale;
            HasLanded = setupData.HasLanded;
            CreationOrder = setupData.CreationOrder;
            MonsterData = setupData.MonsterData;

            orbCollider.radius = Radius;


            shell.Setup(Scale);
            monster.Setup(MonsterData, Radius, Tier);
            SetTint(Color.white);

            switch (setupData.SpawnMode)
            {
                case SoulOrbSpawnMode.Field:
                    SetPhase(SoulOrbPhase.Idle);
                    break;
                case SoulOrbSpawnMode.Loaded:
                    SetPhase(SoulOrbPhase.WaitForShoot);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void TickVisuals(float deltaTime)
        {
            monster.TickVisuals(deltaTime);
            shell.TickVisuals();
        }

        public void OnDespawn()
        {
            ReleaseResources();
        }

        private void OnDestroy()
        {
            ReleaseResources();
        }

        private void ReleaseResources()
        {
            _spawnCts.CancelAndDispose();
            _spawnCts = null;
        }

        public void SetPhase(SoulOrbPhase phase)
        {
            _phase = phase;
            switch (phase)
            {
                case SoulOrbPhase.WaitForShoot:
                    SetSortingLayerID(SortingLayers.SHOOTER);
                    _rb.simulated = false;
                    break;
                case SoulOrbPhase.Idle:
                    SetSortingLayerID(SortingLayers.UNIT);
                    _rb.simulated = true;
                    _rb.bodyType = RigidbodyType2D.Dynamic;
                    break;
                case SoulOrbPhase.MergeAbsorbed:
                    SetSortingLayerID(SortingLayers.MERGE);
                    _rb.simulated = false;
                    break;
                case SoulOrbPhase.MergeAnchored:
                    _rb.simulated = true;
                    _rb.bodyType = RigidbodyType2D.Kinematic;
                    ResetPhysics();
                    break;
                case SoulOrbPhase.Inert:
                    _rb.simulated = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(phase), phase, null);
            }
        }

        public void Shoot(Vector2 velocity, float powerRatio)
        {
            _isActiveShot = true;
            transform.localScale = Vector3.one;
            SetTint(Color.white);
            SetPhase(SoulOrbPhase.Idle);
            _rb.linearVelocity = velocity;

            var spinSign = velocity.x < 0f ? 1f : -1f;
            SpinAsync(spinSign * settings.Spin.Speed * powerRatio, _spawnCts.Token).Forget();
        }

        public void SetTint(Color color)
        {
            shell.SetCaseColor(MonsterData.soulOrbColor * color);
            monster.SetColor(color);
        }

        public void SetOutline(bool value)
        {
            if (value) shell.SetOutlineColor(Color.white);
            else shell.SetOutlineColor(MonsterData.soulOrbColor);
        }

        public void EndActiveShot()
        {
            _isActiveShot = false;
        }


        private void OnCollisionEnter2D(Collision2D other)
        {
            _spinStopRequested = true;

            if (!IsMergeable) return;

            var merged = false;
            if (other.gameObject.TryGetComponent(out SoulOrb otherOrb))
            {
                if (otherOrb.HasLanded) HasLanded = true;

                merged = _tryMerge(this, otherOrb);
            }
            else if (other.gameObject.CompareTag("Ground"))
            {
                HasLanded = true;
            }

            if (!merged)
            {
                var impactSpeed = other.relativeVelocity.magnitude;
                monster.OnCollided(impactSpeed);

                if (_isActiveShot && impactSpeed >= settings.CollisionSfxMinImpact)
                {
                    AudioManager.Instance.PlaySfxOneShot(settings.CollisionSfx);
                }
            }
        }

        private void SetSortingLayerID(int id)
        {
            monster.SetSortingLayerID(id);
            shell.SetSortingLayerID(id);
        }

        private async UniTaskVoid SpinAsync(float spinSpeed, CancellationToken token)
        {
            while (!_spinStopRequested)
            {
                monster.Rotate(spinSpeed * Time.deltaTime);
                await UniTask.Yield(token);
            }

            await DOVirtual.Float(1f, 0f, settings.Spin.StopDelay,
                    t => monster.Rotate(spinSpeed * t * Time.deltaTime))
                .SetEase(Ease.InQuad).Play()
                .AwaitForComplete(TweenCancelBehaviour.KillAndCancelAwait, token);
        }

        private void ResetPhysics()
        {
            _rb.linearVelocity = Vector2.zero;
            _rb.angularVelocity = 0f;
        }
    }
}