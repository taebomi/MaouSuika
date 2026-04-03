using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace TBM.MaouSuika.Core.UI
{
    [RequireComponent(typeof(RawImage))]
    public class ModalBackdrop : MonoBehaviour
    {
        [SerializeField] private float fadeDuration = 0.125f;
        [SerializeField] private float fadeValue = 0.9f;

        private RawImage _rawImage;

        private void Awake()
        {
            _rawImage = GetComponent<RawImage>();
        }

        public async UniTask ShowWithFadeAsync()
        {
            await _rawImage.DOFade(fadeValue, fadeDuration)
                .From(0f)
                .SetUpdate(true)
                .SetEase(Ease.OutQuad)
                .Play().WithCancellation(destroyCancellationToken);
        }

        public async UniTask HideWithFadeAsync()
        {
            await _rawImage.DOFade(0, fadeDuration * 0.5f)
                .SetUpdate(true)
                .SetEase(Ease.InQuad)
                .OnComplete(() => gameObject.SetActive(false))
                .Play().WithCancellation(destroyCancellationToken);
        }
    }
}