using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using MaouSuika.Core;
using MaouSuika.Gameplay.Audio;
using R3;
using TBM.Core;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MaouSuika.Gameplay
{
    public class GameplayRoot : MonoBehaviour
    {
        private enum GameplayPhase
        {
            Loading,
            Playing,
            Paused,
            GameOver,
        }

        [SerializeField] private MonsterDataSO[] monstersByTier;
        [SerializeField] private SkillDefinitionSO defaultSkillDefinition;

        [SerializeField] private PuzzleRoot puzzle;

        [SerializeField] private ScoreRoot score;
        [SerializeField] private PlayerRegionController playerRegion;


        [SerializeField] private GameOverResultView resultView;
        [SerializeField] private PauseUIController pauseUIController;
        [SerializeField] private GameplayAudioController audioController;

        private MonsterLoadout _monsterLoadout;
        private SkillLoadout _skillLoadout;

        private GameplayPlayerInput _playerInput;

        private GameplayPhase _phase = GameplayPhase.Loading;

        private CancellationTokenSource _gameOverCts;


        private void Start()
        {
            Initialize();
            Setup();
            EnterPlaying();
        }

        public void Initialize()
        {
            pauseUIController.Initialize(Pause, Resume, Restart);

            _monsterLoadout = new MonsterLoadout(monstersByTier);
            _skillLoadout = new SkillLoadout(defaultSkillDefinition);

            var playerInput = InputManager.Instance.MainPlayer;
            _playerInput =
                new GameplayPlayerInput(
                        playerInput, puzzle.ScreenToWorldPoint,
                        () => EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                    .AddTo(this);

            score.Initialize();
            puzzle.Initialize(_monsterLoadout, _skillLoadout, _playerInput, score.Add, playerInput.RequestHaptics);

            audioController.Initialize();
            playerRegion.Initialize(audioController.PlayBgm);
            puzzle.GameOverPhase.Subscribe(audioController.SetGameOverPhase).AddTo(this);

            resultView.Initialize(Restart);

            Observable.Merge(puzzle.GameOver).Subscribe(OnGameOver).AddTo(this);
        }


        public void Setup()
        {
            score.Setup();
            puzzle.Setup();
            playerRegion.Setup(score.TotalScoreChanged, score.CurrentScore);
        }

        private void Update()
        {
            switch (_phase)
            {
                case GameplayPhase.Loading:
                    break;
                case GameplayPhase.Playing:
                    TickPlaying(Time.deltaTime);
                    break;
                case GameplayPhase.Paused:
                    TickPaused();
                    break;
                case GameplayPhase.GameOver:
                    TickGameOver(Time.deltaTime);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnDestroy()
        {
            _gameOverCts.CancelAndDispose();
            _gameOverCts = null;

            Time.timeScale = 1f;
        }


        private void EnterPlaying()
        {
            resultView.SetVisible(false, true);
            pauseUIController.ShowPlaying();
            _playerInput.Enable();
            puzzle.EnterPlaying();
            _phase = GameplayPhase.Playing;
        }

        private void Pause()
        {
            if (_phase is not GameplayPhase.Playing) return;

            Time.timeScale = 0f;
            _phase = GameplayPhase.Paused;

            puzzle.Pause();
            pauseUIController.ShowPaused();
        }

        private void Resume()
        {
            if (_phase is not GameplayPhase.Paused) return;

            Time.timeScale = 1f;
            _phase = GameplayPhase.Playing;

            pauseUIController.ShowPlaying();
            puzzle.Resume();
        }

        private void OnGameOver(Unit _)
        {
            if (_phase is not GameplayPhase.Playing) return;

            _phase = GameplayPhase.GameOver;
            pauseUIController.Hide();

            _gameOverCts.CancelAndDispose();
            _gameOverCts = new CancellationTokenSource();

            puzzle.Stop();
            _playerInput.Disable();
            PlayGameOverSequenceAsync(_gameOverCts.Token).Forget();
        }

        private void Restart()
        {
            _gameOverCts.CancelAndDispose();
            _gameOverCts = null;

            _phase = GameplayPhase.Loading;
            Time.timeScale = 1f;

            _playerInput.Disable();
            pauseUIController.Hide();

            SceneFlowManager.Instance.ChangePrimarySceneAsync(SceneNames.GAMEPLAY).Forget();
        }

        private void TickPlaying(float deltaTime)
        {
            if (_playerInput.PausePressedThisFrame)
            {
                Pause();
                return;
            }

            puzzle.TickPlaying(deltaTime);
        }

        private void TickPaused()
        {
            if (!_playerInput.PausePressedThisFrame) return;

            if (!pauseUIController.TryHandleBack())
            {
                Resume();
            }
        }

        private void TickGameOver(float deltaTime)
        {
            puzzle.TickGameOver(deltaTime);
        }

        private async UniTaskVoid PlayGameOverSequenceAsync(CancellationToken token)
        {
            await puzzle.PlayGameOverSequenceAsync(token);
            resultView.SetVisible(true);
        }
    }
}