using UnityEngine;

namespace MaouSuika.Gameplay
{
    public class SingleSoulOrbTargeting
    {
        private const float NAVIGATE_THRESHOLD_SQR = 0.25f;

        private readonly SoulOrbTargetNavigator _navigator;
        private readonly SingleSoulOrbTargetingView _view;

        private bool _canNavigate;

        public SoulOrb Target { get; private set; }

        public SingleSoulOrbTargeting(SoulOrbTargetNavigator navigator, SingleSoulOrbTargetingView view)
        {
            _navigator = navigator;
            _view = view;
        }

        public void Begin()
        {
            Target = null;
            _canNavigate = true;

            _view.Hide();
        }

        public TargetingResult HandleInput(in SkillInputSnapshot input)
        {
            if (input.CancelPressedThisFrame) return TargetingResult.Canceled;

            if (input.Point.HasValue)
            {
                HandlePointer(input.Point.Value);
            }
            else
            {
                HandleNavigation(input.Navigate);
            }

            RefreshView();

            if (input.ConfirmPressedThisFrame && Target != null)
            {
                return TargetingResult.Confirmed;
            }

            return TargetingResult.InProgress;
        }

        public void End()
        {
            _view.Hide();

            Target = null;
            _canNavigate = true;
        }

        private void HandlePointer(Vector2 worldPosition)
        {
            Target = _navigator.ResolveAt(worldPosition);
        }

        private void HandleNavigation(Vector2 direction)
        {
            var hasDirection = direction.sqrMagnitude >= NAVIGATE_THRESHOLD_SQR;
            if (Target == null || !Target.CanBeTargeted)
            {
                Target = _navigator.ResolveInitial();
                _canNavigate = !hasDirection;
                return;
            }

            if (!hasDirection)
            {
                _canNavigate = true;
                return;
            }

            if (!_canNavigate) return;

            Target = _navigator.ResolveNext(Target, direction);
            _canNavigate = false;
        }

        private void RefreshView()
        {
            if (Target != null) _view.Show(Target);
            else _view.Hide();
        }
    }
}