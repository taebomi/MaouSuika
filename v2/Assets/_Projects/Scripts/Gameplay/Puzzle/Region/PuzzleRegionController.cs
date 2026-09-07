using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace MaouSuika.Gameplay
{
    public class PuzzleRegionController : MonoBehaviour
    {
        [SerializeField] private Light2D globalLight;

        [SerializeField] private float fadeDuration = 0.3f;
        [SerializeField] private Ease transitionEase = Ease.InOutSine;

        private PuzzleRegionPresentation _currentPresentation;

        public async UniTask TransitionToAsync(RegionDefinitionSO.PuzzlePresentationSettings settings,
            CancellationToken token)
        {
            var previous = _currentPresentation;
            var next = Instantiate(settings.Prefab, transform);

            next.SetAlpha(0f);

            if (previous == null)
            {
                next.SetAlpha(1f);
                globalLight.intensity = settings.GlobalLightIntensity;
                _currentPresentation = next;
                return;
            }

            await UniTask.WhenAll(
                TransitionGlobalLightToAsync(settings.GlobalLightIntensity, token),
                previous.FadeToAsync(0f, token),
                next.FadeToAsync(1f, token));

            Destroy(previous.gameObject);
            _currentPresentation = next;
        }

        private UniTask TransitionGlobalLightToAsync(float targetIntensity, CancellationToken token)
        {
            return DOTween.To(
                    () => globalLight.intensity,
                    value => globalLight.intensity = value,
                    targetIntensity,
                    fadeDuration)
                .SetEase(transitionEase)
                .Play()
                .ToUniTask(TweenCancelBehaviour.KillAndCancelAwait, cancellationToken: token);
        }
    }
}