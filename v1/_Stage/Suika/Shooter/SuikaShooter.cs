using System;
using UnityEngine;

namespace SOSG.Stage.Suika.Shooter
{
    public class SuikaShooter : MonoBehaviour
    {
        private const float ShootPower = 20f;
        private const float RotateSpeed = 135f;

        public const float CooldownAlpha = 0.333333f;
        public static readonly Color CollidedColor = Color.red;

        public enum State
        {
            None,
            Cooldown,
            Collided,
        }

        [SerializeField] private SuikaShooterShootTimer shootTimer;
        [SerializeField] private SuikaShooterCooldownTimer cooldownTimer;
        [field: SerializeField] public SuikaShooterCollideChecker CollideChecker { get; private set; }
        [SerializeField] private SuikaShooterAim aim;
        [SerializeField] private SuikaCreator suikaCreator;
        [SerializeField] private SuikaQueue queue;

        public SuikaObject LoadedSuika { get; private set; }
        public SuikaObject LastShotSuika { get; private set; }
        public bool CanShoot => CurState is State.None;


        public State CurState { get; private set; }


        public event Action Shot;
        public event Action<State> StateChanged;

        private void Awake()
        {
            CurState  = State.None;
        }

        private void Update()
        {
            if (CurState is State.None && LoadedSuika)
            {
                LoadedSuika.transform.Rotate(0f, 0f, RotateSpeed * Time.deltaTime);
            }
        }

        public void SetUp()
        {
            aim.SetUp();
            shootTimer.SetUp(5f);
        }

        public void OnGameStarted()
        {
            Reload();
            shootTimer.StartTimer();
        }

        public void OnGameOver()
        {
            LoadedSuika = null;
        }

        public void ShootSuika(Vector2 dir, float powerRatio)
        {
            if (CanShoot is false || !LoadedSuika)
            {
                return;
            }

            LastShotSuika = LoadedSuika;

            LoadedSuika.OnShot(dir * (powerRatio * ShootPower));
            Reload();

            cooldownTimer.StartCooldown();
            // todo score 시스템 체크
            Shot?.Invoke();
        }

        public void Reload()
        {
            var tier = queue.Pop();
            var suika = suikaCreator.GetSuika(tier, transform.position);
            suika.OnLoaded();
            LoadedSuika = suika;
        }


        public void OnCooldownEnter()
        {
            ApplyState(State.Cooldown);
        }

        public void OnCooldownExit()
        {
            var state = CollideChecker.IsCollided ? State.Collided : State.None;
            ApplyState(state);
        }

        public void OnCollidedEnter()
        {
            var state = cooldownTimer.IsCooldown ? State.Cooldown : State.Collided;
            ApplyState(state);
        }

        public void OnCollidedExit()
        {
            var state = cooldownTimer.IsCooldown ? State.Cooldown : State.None;
            ApplyState(state);
        }

        private void ApplyState(State state)
        {
            if (CurState == state)
            {
                return;
            }

            switch (state)
            {
                case State.None:
                    LoadedSuika.VisualComponent.SetAlphaWithCapsuleColor(1f);
                    break;
                case State.Cooldown:
                    LoadedSuika.VisualComponent.SetAlphaWithCapsuleColor(CooldownAlpha);
                    break;
                case State.Collided:
                    LoadedSuika.VisualComponent.SetColor(CollidedColor, true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            aim.SetState(state);
            CurState = state;
            StateChanged?.Invoke(state);
        }
    }
}