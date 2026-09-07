using R3;
using UnityEngine;

namespace MaouSuika.Gameplay
{
    public class SoulOrbShooterAimView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer body;
        [SerializeField] private SpriteRenderer tail;

        private SoulOrbShooterSettingsSO _settings;
        private ShooterVisualState _visualState;
        private AimData? _aimData;

        public void Initialize(SoulOrbShooterViewModel viewModel, SoulOrbShooterSettingsSO settings)
        {
            _settings = settings;
            
            viewModel.VisualState.Subscribe(OnStateChanged).AddTo(this);
            viewModel.Aim.Subscribe(OnAimChanged).AddTo(this);
        }

        public void Setup()
        {
            gameObject.SetActive(false);
        }

        private void OnStateChanged(ShooterVisualState visualState)
        {
            _visualState = visualState;
            Render();
        }

        private void OnAimChanged(AimData? aim)
        {
            _aimData = aim;
            Render();
        }

        private void Render()
        {
            if (_aimData == null)
            {
                gameObject.SetActive(false);
                return;
            }

            gameObject.SetActive(true);

            var visual = _settings.EvaluateAim(_aimData.Value, _visualState);

            transform.right = _aimData.Value.Direction;
            body.size = visual.Size;
            body.color = visual.Color;
            tail.color = visual.Color;
        }
    }
}