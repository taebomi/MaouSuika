using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using TBM.MaouSuika.Core.Input;
using TBM.MaouSuika.Core.Scene;
using TBM.MaouSuika.Gameplay;
using TBM.MaouSuika.Gameplay.Audio;
using TBM.MaouSuika.Gameplay.Battle;
using TBM.MaouSuika.Gameplay.Player;
using TBM.MaouSuika.Gameplay.Puzzle;
using TBM.MaouSuika.Scenes.Gameplay;
using UnityEngine;

namespace TBM.MaouSuika.Scenes.Gameplay
{
    public class GameplayManager : SceneControllerBase
    {
        [SerializeField] private PlayerContextFactorySO playerContextFactory;

        [SerializeField] private GameOverEventChannelSO gameOverEventChannel;

        [SerializeField] private PuzzleController puzzleController;
        [SerializeField] private BattleController battleController;

        [SerializeField] private GameplayBgmController gameplayBgmController;
        [SerializeField] private GlobalDangerAudioSystem globalDangerAudioSystem;

        [SerializeField] private GameOverUI gameOverUI;

        private GameplayPayload _payload;
        private PlayerContext _playerContext;

        private Coroutine _gameplayLoop;

        private void OnEnable()
        {
            RegisterRoundEvents();
        }

        public override async UniTask InitializeSceneAsync(object initPayload)
        {
            if (initPayload is not GameplayPayload payload)
            {
                throw new ArgumentException($"{nameof(initPayload)} must be {nameof(GameplayPayload)}");
            }

            _payload = payload;
            Initialize(payload);
            Setup();

            await UniTask.CompletedTask;
        }

        public override void ProcessScene()
        {
            StartRound();
        }

        private void OnDisable()
        {
            UnregisterRoundEvents();
        }

        private void Initialize(GameplayPayload payload)
        {
            _playerContext = playerContextFactory.Create(0, payload.MonsterLoadoutData);

            globalDangerAudioSystem.Initialize();

            puzzleController.Initialize(_playerContext);
            battleController.Initialize(_playerContext);

            InputManager.Instance.SwitchMap(InputConstants.Map.GAMEPLAY);
        }

        private void Setup()
        {
            puzzleController.Setup();
            battleController.Setup();
        }

        private void StartRound()
        {
            gameplayBgmController.PlayBgm();
            puzzleController.HandleGameStarted();

            _gameplayLoop = StartCoroutine(RunRoundRoutine());
        }

        private IEnumerator RunRoundRoutine()
        {
            while (true)
            {
                var deltaTime = Time.deltaTime;

                puzzleController.Tick(deltaTime);
                battleController.Tick(deltaTime);

                yield return null;
            }
        }

        private void OnGameOver(int playerIndex)
        {
            if (_gameplayLoop != null)
            {
                StopCoroutine(_gameplayLoop);
                _gameplayLoop = null;
            }

            StartCoroutine(PlayGameOverRoutine(playerIndex));
        }

        private IEnumerator PlayGameOverRoutine(int playerIndex)
        {
            var puzzleRoutine = StartCoroutine(puzzleController.PlayGameOverRoutine());
            var battleRoutine = StartCoroutine(battleController.PlayGameOverSequence());

            yield return puzzleRoutine;
            yield return battleRoutine;

            gameOverUI.Show(OnRetryRequested);
        }

        private void OnRetryRequested()
        {
            SceneManager.Instance.LoadScene(SceneNames.GAMEPLAY, new SceneLoadOptions
            {
                Payload =
                    new SceneLoadPayload(null, _payload)
            });
        }

        private void RegisterRoundEvents()
        {
            gameOverEventChannel.EventRaised += OnGameOver;
            puzzleController.MergeActionFinished += battleController.OnPuzzleMergeActionFinished;
            puzzleController.ScoreChanged += battleController.OnScoreChanged;
        }

        private void UnregisterRoundEvents()
        {
            gameOverEventChannel.EventRaised -= OnGameOver;
            puzzleController.MergeActionFinished -= battleController.OnPuzzleMergeActionFinished;
            puzzleController.ScoreChanged -= battleController.OnScoreChanged;
        }
    }
}