using System;
using System.Collections;
using TBM.MaouSuika.Core;
using TBM.MaouSuika.Core.Input;
using TBM.MaouSuika.Gameplay.Player;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    public class PuzzleController : MonoBehaviour
    {
        [SerializeField] private SuikaTierConfigSO suikaTierConfig;

        private PuzzleInputSystem _inputSystem;

        [SerializeField] private SuikaSystem suikaSystem;
        [SerializeField] private SuikaQueueSystem queueSystem;
        [SerializeField] private MergeSystem mergeSystem;

        [SerializeField] private ShooterSystem shooterSystem;
        [SerializeField] private SkillSystem skillSystem;

        [SerializeField] private ScoreSystem scoreSystem;
        [SerializeField] private ComboSystem comboSystem;

        [SerializeField] private PuzzleArea area;
        [SerializeField] private PuzzleGameOverSystem gameOverSystem;

        [SerializeField] private PuzzleRegionSystem regionSystem;


        private readonly InputLockSystem _inputLockSystem = new();

        private PuzzleContext _context;

        /// <summary>
        /// Merge 작업이 완전히 완료된 이후, Merge된 Tier 알림 
        /// </summary>
        public event Action<int> MergeActionFinished;
        /// <summary>
        /// 점수가 변경될 때 발행. 인자: 현재 누적 점수
        /// </summary>
        public event Action<int> ScoreChanged;
        
        public void Initialize(PlayerContext playerContext)
        {
            _context = CreateContext(playerContext);

            var inputController = InputManager.Instance.GetInputController(playerContext.PlayerIndex);
            _inputSystem = new PuzzleInputSystem(inputController);

            suikaSystem.Initialize(mergeSystem, _context.TierDataTable);
            queueSystem.Initialize(_context.TierDataTable);

            shooterSystem.Initialize(suikaSystem, queueSystem, inputController, area);
            // skillSystem.Initialize(new SkillExecutionContext
            // {
            //     SuikaSystem = suikaSystem,
            //     InputController = inputController,
            //     InputLockSystem = _inputLockSystem,
            //     Camera = Camera.main,
            // });

            scoreSystem.Initialize(_context.TierDataTable);

            gameOverSystem.Initialize(_context.PlayerContext.PlayerIndex, shooterSystem);

            RegisterEvents();
        }

        private PuzzleContext CreateContext(PlayerContext playerContext)
        {
            return new PuzzleContext.Builder()
                .SetPlayerContext(playerContext)
                .SetArea(area)
                .SetTierConfig(suikaTierConfig)
                .Build();
        }

        public void Setup()
        {
            _inputLockSystem.Lock(InputConstants.LockReason.INITIALIZATION, InputChannel.ShooterFire | InputChannel.UI);

            suikaSystem.Setup();
            scoreSystem.Setup();
            queueSystem.Setup(GameRule.Puzzle.Queue.DEFAULT_VISIBLE_COUNT); // todo 특성 추가 시 변경 필요
            mergeSystem.Setup();

            comboSystem.Setup();
            // skillSystem.Setup();

            shooterSystem.Setup();

            gameOverSystem.ResetSystem();

            regionSystem.Setup();
        }

        private void OnDestroy()
        {
            UnregisterEvents();
        }

        // ################################################

        // Game State

        // ################################################

        public void HandleGameStarted()
        {
            _inputLockSystem.Clear();

            shooterSystem.HandleGameStarted();
        }

        public void Tick(float deltaTime)
        {
            gameOverSystem.Tick(deltaTime);
            if (gameOverSystem.IsGameOver) return;


            var inputResult = _inputSystem.ReadInput();
            shooterSystem.Tick(inputResult.Shooter, Time.deltaTime);

            // if (inputResult.Skill.SkillRequestedThisFrame)
            //     skillSystem.TryActivate();
        }


        public IEnumerator PlayGameOverRoutine()
        {
            _inputLockSystem.Lock("GameOver", InputChannel.All);

            shooterSystem.HandleGameOver();
            gameOverSystem.Deactivate();

            yield return PlayGameOverSequence();
        }

        private IEnumerator PlayGameOverSequence()
        {
            // todo : SuikaSystem으로부터 받아서 연출
            Logger.Info($"GameOver 연출");
            yield return null;
        }

        // ################################################
        // Events
        // ################################################
        private void RegisterEvents()
        {
            mergeSystem.MergeActionFinished += OnSuikaMerged;
            shooterSystem.Fired += OnShooterFired;
            comboSystem.ComboTriggered += OnComboTriggered;
            scoreSystem.ScoreChanged += OnScoreChanged;
        }

        private void UnregisterEvents()
        {
            mergeSystem.MergeActionFinished -= OnSuikaMerged;
            shooterSystem.Fired -= OnShooterFired;
            comboSystem.ComboTriggered -= OnComboTriggered;
            scoreSystem.ScoreChanged -= OnScoreChanged;
        }

        // ################################################
        // Callbacks
        // ################################################
        private void OnSuikaMerged(SuikaObject sourceSuika, SuikaObject targetSuika, MergeEvent mergeEvent)
        {
            suikaSystem.HandleSuikaMerged(sourceSuika, targetSuika, mergeEvent);
            comboSystem.HandleSuikaMerged(mergeEvent);
            scoreSystem.HandleSuikaMerged(mergeEvent);
            mergeSystem.PlayMergeFeedback(mergeEvent, comboSystem.CurCombo);

            MergeActionFinished?.Invoke(mergeEvent.Tier);
        }

        private void OnShooterFired()
        {
            comboSystem.HandleShooterFired();
        }

        private void OnComboTriggered(ComboEvent comboEvent)
        {
            // skillSystem.HandleComboTriggered(comboEvent);
        }

        private void OnScoreChanged(int score)
        {
            regionSystem.TransitionByScore(score);
            ScoreChanged?.Invoke(score);
        }
    }
}