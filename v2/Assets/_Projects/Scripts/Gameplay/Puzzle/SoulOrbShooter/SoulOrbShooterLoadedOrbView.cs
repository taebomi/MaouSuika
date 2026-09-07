using DG.Tweening;
using R3;
using UnityEngine;

namespace MaouSuika.Gameplay
{
    public class SoulOrbShooterLoadedOrbView : MonoBehaviour
    {
        private SoulOrbShooterSettingsSO _settings;
        private SoulOrbShooterViewModel _viewModel;

        private Sequence _reloadSequence;

        public void Initialize(SoulOrbShooterViewModel viewModel, SoulOrbShooterSettingsSO settings)
        {
            _settings = settings;
            _viewModel = viewModel;
            _viewModel.VisualState.Subscribe(OnStateChanged).AddTo(this);
            _viewModel.Reloaded.Subscribe(PlayReload).AddTo(this);
        }


        public void Tick(float deltaTime)
        {
            var loadedOrb = _viewModel.LoadedSoulOrb;
            if (!loadedOrb) return;

            loadedOrb.transform.Rotate(0f, 0f, _settings.LoadedOrb.SpinSpeed * deltaTime);
        }

        public void Stop()
        {
            _reloadSequence?.Kill(true);
            _reloadSequence = null;
        }

        private void OnStateChanged(ShooterVisualState visualState)
        {
            var loadedOrb = _viewModel.LoadedSoulOrb;
            if (!loadedOrb) return;

            loadedOrb.SetTint(_settings.EvaluateLoadedOrbTint(visualState));
        }


        private void PlayReload(SoulOrb reloadedOrb)
        {
            if (!reloadedOrb) return;

            _reloadSequence?.Kill();

            var duration = _settings.Fire.FireCooldown;
            if (duration <= 0f) return;

            var targetTr = reloadedOrb.transform;
            var targetPos = targetTr.position;
            var baseScale = targetTr.localScale;

            _reloadSequence = DOTween.Sequence()
                .Append(targetTr.DOMove(targetPos, duration * 0.6f)
                    .From(targetPos + Vector3.down * _settings.LoadedOrb.RiseOffset)
                    .SetEase(Ease.OutCubic))
                .Join(targetTr.DOScale(baseScale, duration)
                    .From(baseScale * _settings.LoadedOrb.PopStartScale)
                    .SetEase(Ease.OutBack, _settings.LoadedOrb.PopOvershoot))
                .SetLink(reloadedOrb.gameObject)
                .Play();
        }
    }
}