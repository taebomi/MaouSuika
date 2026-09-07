using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MaouSuika.Gameplay.Skill
{
    public class PointerSkillInputReader : ISkillInputReader
    {
        private readonly InputAction _point;
        private readonly InputAction _confirm;
        private readonly InputAction _cancel;

        private readonly Func<Vector2, Vector2> _screenToWorld;
        private readonly Func<bool> _isPointerOverUI;


        public PointerSkillInputReader(
            InputAction point, InputAction confirm, InputAction cancel,
            Func<Vector2, Vector2> screenToWorld, Func<bool> isPointerOverUI)
        {
            _point = point;
            _confirm = confirm;
            _cancel = cancel;
            _screenToWorld = screenToWorld;
            _isPointerOverUI = isPointerOverUI;
        }

        public SkillInputSnapshot ReadInput()
        {
            var point = _screenToWorld(_point.ReadValue<Vector2>());

            var confirmPressed = _confirm.WasPressedThisFrame() && !_isPointerOverUI();
            return new SkillInputSnapshot(
                point, Vector2.zero, confirmPressed, _cancel.WasPressedThisFrame());
        }
    }
}