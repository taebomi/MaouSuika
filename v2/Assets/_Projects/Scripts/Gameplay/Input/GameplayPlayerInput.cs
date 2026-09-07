using System;
using MaouSuika.Core;
using MaouSuika.Gameplay.Shooter;
using MaouSuika.Gameplay.Skill;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MaouSuika.Gameplay
{
    public class GameplayPlayerInput : IDisposable
    {
        private enum InputContext
        {
            None,
            Puzzle,
            Skill,
        }

        private readonly LocalPlayerInput _player;

        private readonly InputActionMap _commonMap;
        private readonly InputActionMap _puzzleMap;
        private readonly InputActionMap _skillMap;

        private readonly InputAction _pauseAction;

        private IShooterInputReader _shooterInputReader;
        private readonly PointerShooterInputReader _pointerShooterInputReader;
        private readonly GamepadShooterInputReader _gamepadShooterInputReader;
        private readonly InputAction _useSkillAction;

        private ISkillInputReader _skillInputReader;
        private readonly PointerSkillInputReader _pointerSkillInputReader;
        private readonly GamepadSkillInputReader _gamepadSkillInputReader;


        private InputContext _context;
        private DisposableBag _disposableBag;

        private readonly Subject<ShooterControlSchemeSettings> _shooterSettingsChanged = new();

        public Observable<ShooterControlSchemeSettings> ShooterSettingsChanged => _shooterSettingsChanged;

        public ShooterControlSchemeSettings CurrentShooterSettings =>
            _player.Profile.GetSettings(_player.CurrentControlScheme);

        public bool PausePressedThisFrame => _pauseAction.WasPressedThisFrame();

        public GameplayPlayerInput(
            LocalPlayerInput player, Func<Vector2, Vector2> screenToWorld, Func<bool> isPointerOverUI)
        {
            _player = player;

            _context = InputContext.None;
            _commonMap = player.GetActionMap(InputNames.ActionMaps.COMMON);
            _commonMap.Disable();
            _puzzleMap = player.GetActionMap(InputNames.ActionMaps.PUZZLE);
            _puzzleMap.Disable();
            _skillMap = player.GetActionMap(InputNames.ActionMaps.SKILL);
            _skillMap.Disable();

            _pauseAction = player.GetAction(InputNames.ActionMaps.COMMON, InputNames.CommonActions.PAUSE);

            var aimAction = player.GetAction(
                InputNames.ActionMaps.PUZZLE, InputNames.PuzzleActions.AIM);
            var fireAction = player.GetAction(
                InputNames.ActionMaps.PUZZLE, InputNames.PuzzleActions.FIRE);
            _useSkillAction = player.GetAction(
                InputNames.ActionMaps.PUZZLE, InputNames.PuzzleActions.USE_SKILL);

            var pointAction = player.GetAction(
                InputNames.ActionMaps.SKILL, InputNames.SkillActions.POINT);
            var navigateAction = player.GetAction(
                InputNames.ActionMaps.SKILL, InputNames.SkillActions.NAVIGATE);
            var confirmAction = player.GetAction(
                InputNames.ActionMaps.SKILL, InputNames.SkillActions.CONFIRM);
            var cancelAction = player.GetAction(
                InputNames.ActionMaps.SKILL, InputNames.SkillActions.CANCEL);

            _pointerShooterInputReader = new PointerShooterInputReader(
                aimAction, fireAction, screenToWorld, isPointerOverUI);
            _gamepadShooterInputReader = new GamepadShooterInputReader(aimAction, fireAction);
            _pointerSkillInputReader = new PointerSkillInputReader(
                pointAction, confirmAction, cancelAction, screenToWorld, isPointerOverUI);
            _gamepadSkillInputReader = new GamepadSkillInputReader(
                navigateAction, confirmAction, cancelAction);

            ApplyControlScheme(player.CurrentControlScheme);

            _shooterSettingsChanged.AddTo(ref _disposableBag);
            player.ControlSchemeChanged.Subscribe(OnControlSchemeChanged).AddTo(ref _disposableBag);
            player.InputProfileChanged.Subscribe(_ => PublishShooterSettings()).AddTo(ref _disposableBag);
        }

        public void Dispose()
        {
            Disable();

            _disposableBag.Dispose();
        }

        public PuzzleInputSnapshot ReadPuzzleInput()
        {
            var shooterInput = _shooterInputReader.ReadInput();
            var skillUsePressed = _useSkillAction.WasPressedThisFrame();

            return new PuzzleInputSnapshot(shooterInput, skillUsePressed);
        }

        public SkillInputSnapshot ReadSkillInput()
        {
            return _skillInputReader.ReadInput();
        }

        public void Enable()
        {
            _commonMap.Enable();
        }

        public void Disable()
        {
            _commonMap.Disable();
            _puzzleMap.Disable();
            _skillMap.Disable();

            _context = InputContext.None;
        }

        public void ClearContext()
        {
            _puzzleMap.Disable();
            _skillMap.Disable();

            _context = InputContext.None;
        }

        public void SwitchToPuzzle()
        {
            if (_context is InputContext.Puzzle) return;

            _skillMap.Disable();
            _puzzleMap.Enable();

            _context = InputContext.Puzzle;
        }

        public void SwitchToSkill()
        {
            if (_context is InputContext.Skill) return;

            _puzzleMap.Disable();
            _skillMap.Enable();

            _context = InputContext.Skill;
        }

        private void OnControlSchemeChanged(string controlScheme)
        {
            ApplyControlScheme(controlScheme);
            PublishShooterSettings();
        }

        private void ApplyControlScheme(string controlScheme)
        {
            switch (controlScheme)
            {
                case InputNames.ControlSchemes.KEYBOARD_MOUSE:
                    _shooterInputReader = _pointerShooterInputReader;
                    _skillInputReader = _pointerSkillInputReader;
                    break;
                case InputNames.ControlSchemes.GAMEPAD:
                    _shooterInputReader = _gamepadShooterInputReader;
                    _skillInputReader = _gamepadSkillInputReader;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(controlScheme));
            }
        }

        private void PublishShooterSettings()
        {
            _shooterSettingsChanged.OnNext(CurrentShooterSettings);
        }
    }
}