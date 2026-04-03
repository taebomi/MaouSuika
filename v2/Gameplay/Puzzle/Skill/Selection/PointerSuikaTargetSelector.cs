using TBM.MaouSuika.Core.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    public class PointerSuikaTargetSelector : ISuikaTargetSelector
    {
        public SuikaObject HoveredSuika { get; private set; }
        public bool IsConfirmed { get; private set; }
        public bool IsCancelled { get; private set; }

        private readonly Camera _camera;
        private readonly string _scheme;

        public PointerSuikaTargetSelector(Camera camera, string scheme)
        {
            _camera = camera;
            _scheme = scheme;
        }

        public void Tick(float deltaTime)
        {
            var screenPos = GetPointerScreenPosition();
            var worldPos = _camera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, _camera.nearClipPlane));

            var collider = Physics2D.OverlapPoint(worldPos);
            HoveredSuika = collider != null && collider.TryGetComponent<SuikaObject>(out var suika) ? suika : null;

            if (WasPressedThisFrame())
            {
                if (HoveredSuika != null)
                    IsConfirmed = true;
                else
                    IsCancelled = true;
            }
        }

        private Vector2 GetPointerScreenPosition()
        {
            if (_scheme == InputConstants.ControlScheme.TOUCHSCREEN)
                return Touchscreen.current?.primaryTouch.position.ReadValue() ?? Vector2.zero;

            return Mouse.current?.position.ReadValue() ?? Vector2.zero;
        }

        private bool WasPressedThisFrame()
        {
            if (_scheme == InputConstants.ControlScheme.TOUCHSCREEN)
                return Touchscreen.current?.primaryTouch.press.wasPressedThisFrame ?? false;

            return Mouse.current?.leftButton.wasPressedThisFrame ?? false;
        }

        public void Dispose() { }
    }
}
