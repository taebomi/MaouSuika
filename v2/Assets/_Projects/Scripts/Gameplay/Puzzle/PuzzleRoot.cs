using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MaouSuika.Core;
using R3;
using UnityEngine;

namespace MaouSuika.Gameplay
{
    public class PuzzleRoot : MonoBehaviour
    {
        [SerializeField] private SoulOrbRoot soulOrbRoot;
        [SerializeField] private SoulOrbQueueRoot soulOrbQueueRoot;
        [SerializeField] private ShooterRoot shooterRoot;
        [SerializeField] private ComboRoot comboRoot;
        [SerializeField] private SkillRoot skillRoot;
        [SerializeField] private PuzzleGameOverRoot gameOverRoot;

        [SerializeField] private GameplayCameraController cameraController;

        private GameplayPlayerInput _playerInput;

        private Action<int> _addScore;

        public ReadOnlyReactiveProperty<GameOverPhase> GameOverPhase => gameOverRoot.Phase;
        public Observable<Unit> GameOver => gameOverRoot.GameOver;

        public Vector2 ScreenToWorldPoint(Vector2 screenPoint) => cameraController.ScreenToWorldPoint(screenPoint);

        public void Initialize(
            MonsterLoadout monsterLoadout,
            SkillLoadout skillLoadout,
            GameplayPlayerInput playerInput,
            Action<int> addScore,
            Action<HapticRequest> requestHaptics)
        {
            _playerInput = playerInput;

            soulOrbQueueRoot.Initialize(monsterLoadout);
            soulOrbRoot.Initialize(monsterLoadout, cameraController.RequestShake, requestHaptics);
            shooterRoot.Initialize(soulOrbRoot.Spawner, playerInput);
            gameOverRoot.Initialize();

            skillRoot.Initialize(soulOrbRoot, comboRoot, shooterRoot, skillLoadout);

            _addScore = addScore;

            shooterRoot.Fired.Subscribe(OnShooterFired).AddTo(this);
            soulOrbRoot.MergeFinished.Subscribe(OnSoulOrbMerged).AddTo(this);
            soulOrbRoot.GameOverOrbPopped.Subscribe(OnGameOverSoulOrbPopped).AddTo(this);
        }


        public void Setup()
        {
            cameraController.Setup();
            soulOrbRoot.Setup();
            skillRoot.Setup();

            var seed = Guid.NewGuid().GetHashCode();
            Debug.Log($"[Maou Suika] Queue Seed: {seed}");
            soulOrbQueueRoot.Setup(seed);

            comboRoot.Setup();
            shooterRoot.Setup(soulOrbQueueRoot.Dequeue);
            gameOverRoot.Setup();
        }

        public void EnterPlaying()
        {
            _playerInput.SwitchToPuzzle();
        }

        public void TickPlaying(float deltaTime)
        {
            soulOrbRoot.TickPlaying(deltaTime);

            if (skillRoot.IsAwaitingInput) TickSkillInput();
            else TickPuzzleInput(deltaTime);

            gameOverRoot.Tick(shooterRoot.IsOverflowing, deltaTime);
        }

        private void TickPuzzleInput(float deltaTime)
        {
            var input = _playerInput.ReadPuzzleInput();
            if (input.UseSkillPressedThisFrame) skillRoot.RequestUse();

            var useResult = skillRoot.ProcessUseRequest();
            if (useResult is SkillUseResult.AwaitingInput)
            {
                shooterRoot.ResetInput();
                _playerInput.SwitchToSkill();
                return;
            }

            shooterRoot.Tick(in input.Shooter, deltaTime);
        }

        private void TickSkillInput()
        {
            var input = _playerInput.ReadSkillInput();
            skillRoot.HandleInput(in input);

            if (skillRoot.IsAwaitingInput) return;

            _playerInput.SwitchToPuzzle();
        }

        public void TickGameOver(float deltaTime)
        {
            soulOrbRoot.TickGameOver(deltaTime);
        }

        public void Pause()
        {
            shooterRoot.ResetInput();
            skillRoot.Pause();

            _playerInput.ClearContext();
        }

        public void Resume()
        {
            skillRoot.Resume();

            _playerInput.SwitchToPuzzle();
        }

        public void Stop()
        {
            skillRoot.Stop();
            soulOrbRoot.Stop();
            shooterRoot.Stop();
        }

        public async UniTask PlayGameOverSequenceAsync(CancellationToken token)
        {
            await soulOrbRoot.PlayGameOverSequenceAsync(token);
        }

        private void OnShooterFired(Unit _)
        {
            comboRoot.CheckPoint();
        }

        private void OnSoulOrbMerged(SoulOrbMergeResult result)
        {
            AddScoreFor(result.SourceTier);
            var combo = comboRoot.Add(result.Position);
            skillRoot.AddChargeForCombo(combo);

            soulOrbRoot.PlayMergeEffect(result, combo);
        }

        private void OnGameOverSoulOrbPopped(SoulOrb soulOrb)
        {
            AddScoreFor(soulOrb.Tier);
        }

        private void AddScoreFor(int tier)
        {
            _addScore(1 << tier);
        }
    }
}
