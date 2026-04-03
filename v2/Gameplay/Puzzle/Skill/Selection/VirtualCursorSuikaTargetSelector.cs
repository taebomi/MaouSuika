using TBM.MaouSuika.Core.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    public class VirtualCursorSuikaTargetSelector : ISuikaTargetSelector
    {
        public SuikaObject HoveredSuika { get; private set; }
        public bool IsConfirmed { get; private set; }
        public bool IsCancelled { get; private set; }

        private readonly Camera _camera;
        private readonly SuikaSelectionVirtualCursor _cursor;
        private readonly float _cursorMoveSpeed;
        private readonly InputAction _aimAction;
        private readonly InputAction _fireAction;
        private readonly InputAction _skillAction;

        private Vector3 _cursorWorldPos;

        public VirtualCursorSuikaTargetSelector(
            Camera camera,
            InputController inputController,
            SuikaSelectionVirtualCursor cursor,
            float cursorMoveSpeed)
        {
            _camera = camera;
            _cursor = cursor;
            _cursorMoveSpeed = cursorMoveSpeed;

            _aimAction = inputController.GetAction(InputConstants.Map.GAMEPLAY, InputConstants.Gameplay.AIM);
            _fireAction = inputController.GetAction(InputConstants.Map.GAMEPLAY, InputConstants.Gameplay.FIRE);
            _skillAction = inputController.GetAction(InputConstants.Map.GAMEPLAY, InputConstants.Gameplay.SKILL);

            _cursorWorldPos = camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, camera.nearClipPlane));
            _cursor.SetPosition(_cursorWorldPos);
            _cursor.Show();
        }

        public void Tick(float deltaTime)
        {
            var aimInput = _aimAction?.ReadValue<Vector2>() ?? Vector2.zero;
            _cursorWorldPos += new Vector3(aimInput.x, aimInput.y, 0f) * (_cursorMoveSpeed * deltaTime);
            _cursor.SetPosition(_cursorWorldPos);

            var collider = Physics2D.OverlapPoint(_cursorWorldPos);
            HoveredSuika = collider != null && collider.TryGetComponent<SuikaObject>(out var suika) ? suika : null;

            if (_fireAction != null && _fireAction.WasPressedThisFrame())
            {
                if (HoveredSuika != null)
                    IsConfirmed = true;
                else
                    IsCancelled = true;
            }

            if (_skillAction != null && _skillAction.WasPressedThisFrame())
                IsCancelled = true;
        }

        public void Dispose()
        {
            _cursor.Hide();
        }
    }
}
