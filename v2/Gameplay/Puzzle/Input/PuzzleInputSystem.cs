using System;
using TBM.MaouSuika.Core.Input;
using TBM.MaouSuika.Core.Settings;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    public class PuzzleInputSystem : IDisposable
    {
        private readonly ShooterInputHandler _shooterInputHandler;

        private InputController _inputController;

        private InputProfile Profile => _inputController.Profile;

        public PuzzleInputSystem(InputController inputController)
        {
            _inputController = inputController;

            _shooterInputHandler = new ShooterInputHandler(_inputController);

            UpdateHandlerStates();

            _inputController.ControlSchemeChanged += OnControlSchemeChanged;
            _inputController.ProfileChanged += OnInputProfileChanged;
        }

        public void Dispose()
        {
            if (_inputController != null)
            {
                _inputController.ControlSchemeChanged -= OnControlSchemeChanged;
                _inputController.ProfileChanged -= OnInputProfileChanged;
            }
        }

        public PuzzleInputResult ReadInput()
        {
            return new PuzzleInputResult()
            {
                Shooter = _shooterInputHandler.ReadInput(),
                Skill = ReadSkillInput(),
            };
        }

        private SkillInputResult ReadSkillInput()
        {
            var action = _inputController.GetAction(InputConstants.Map.GAMEPLAY, InputConstants.Gameplay.SKILL);
            return new SkillInputResult()
            {
                SkillRequestedThisFrame = action != null && action.triggered,
                InputType = GetSkillInputType(_inputController.Scheme),
            };
        }

        private static SkillInputType GetSkillInputType(string scheme)
        {
            return scheme switch
            {
                InputConstants.ControlScheme.MOUSE => SkillInputType.Pointer,
                InputConstants.ControlScheme.KEYBOARD_MOUSE => SkillInputType.Pointer,
                InputConstants.ControlScheme.TOUCHSCREEN => SkillInputType.Pointer,
                _ => SkillInputType.VirtualCursor,
            };
        }

        private void OnControlSchemeChanged(string scheme)
        {
            UpdateHandlerStates();
        }

        private void OnInputProfileChanged(InputProfile profile)
        {
            UpdateHandlerStates();
        }

        private void UpdateHandlerStates()
        {
            var profile = _inputController.Profile;
            var scheme = _inputController.Scheme;

            var inputType = profile.GetShooterInputType(scheme);
            _shooterInputHandler.SetInputType(inputType);
            _shooterInputHandler.SetDeadZone(profile.GetDeadZone(scheme));
        }
    }
}