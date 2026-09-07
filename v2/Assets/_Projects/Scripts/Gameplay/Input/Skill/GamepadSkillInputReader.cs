using UnityEngine;
using UnityEngine.InputSystem;

namespace MaouSuika.Gameplay.Skill
{
    public class GamepadSkillInputReader : ISkillInputReader
    {
        private readonly InputAction _navigate;
        private readonly InputAction _confirm;
        private readonly InputAction _cancel;

        public GamepadSkillInputReader(
            InputAction navigate,
            InputAction confirm,
            InputAction cancel)
        {
            _navigate = navigate;
            _confirm = confirm;
            _cancel = cancel;
        }

        public SkillInputSnapshot ReadInput()
        {
            return new SkillInputSnapshot(
                null,
                _navigate.ReadValue<Vector2>(),
                _confirm.WasPressedThisFrame(),
                _cancel.WasPressedThisFrame());
        }
    }
}