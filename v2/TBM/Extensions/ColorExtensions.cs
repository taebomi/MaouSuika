using UnityEngine;

namespace TBM.Extensions
{
    public static class ColorExtensions
    {
        /// <summary>
        /// hsv 기준으로 Color값을 변경
        /// </summary>
        /// <param name="color"></param>
        /// <param name="hDelta"></param>
        /// <param name="sDelta"></param>
        /// <param name="vDelta"></param>
        /// <returns></returns>
        public static Color AdjustHSV(this Color color, float hDelta, float sDelta, float vDelta)
        {
            Color.RGBToHSV(color, out var h, out var s, out var v);
            h = Mathf.Clamp(h + hDelta, 0f, 1f);
            s = Mathf.Clamp(s + sDelta, 0f, 1f);
            v = Mathf.Clamp(v + vDelta, 0f, 1f);
            return Color.HSVToRGB(h, s, v);
        }

        /// <summary>
        /// rgb값은 유지하며 alpha 값만 변경
        /// </summary>
        public static Color WithAlpha(this Color color, float alpha) => new(color.r, color.g, color.b, alpha);
    }
}