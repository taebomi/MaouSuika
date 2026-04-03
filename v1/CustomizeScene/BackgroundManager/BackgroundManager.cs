using UnityEngine;
using UnityEngine.UI;

namespace SOSG.Customization
{
    public class BackgroundManager : MonoBehaviour
    {
        [SerializeField] private RawImage backgroundRawImage;

        [SerializeField] private float scrollSpeed = 0.05f;

        private void AdjustRatio(float ratio)
        {
            // var rect = backgroundRawImage.uvRect;
            // const float renderTextureRatio = 1f;
            // rect.width = renderTextureRatio / ratio;
            // backgroundRawImage.uvRect = rect;
        }

        private void Update()
        {
            var rect = backgroundRawImage.uvRect;
            var scroll = scrollSpeed * Time.deltaTime;
            rect.x += scroll;
            rect.y += scroll;
            backgroundRawImage.uvRect = rect;
        }
    }
}