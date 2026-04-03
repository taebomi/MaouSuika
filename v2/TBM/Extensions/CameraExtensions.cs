using UnityEngine;

namespace TBM.Extensions
{
    public static class CameraExtensions
    {
        public static bool IsInside(this Camera cam, Vector2 screenPoint)
        {
            return cam.pixelRect.Contains(screenPoint);
        }
    }
}