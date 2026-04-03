using System.Runtime.CompilerServices;
using UnityEngine;

namespace TBM.Core
{
    public static class Easing
    {
        private const float HALF_PI = Mathf.PI * 0.5f;
        private const float TWO_PI = Mathf.PI * 2f;

        // Back Constant
        private const float C1 = 1.70158f;
        private const float C2 = C1 * 1.525f;
        private const float C3 = C1 + 1f;

        // Elastic Constants
        private const float C4 = TWO_PI / 3f;
        private const float C5 = TWO_PI / 4.5f;

        // Bounce Constants
        private const float N1 = 7.5625f;
        private const float D1 = 2.75f;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Linear(float t) => t;

        #region Sine

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float InSine(float t) => 1f - Mathf.Cos(t * HALF_PI);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float OutSine(float t) => Mathf.Sin(t * HALF_PI);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float InOutSine(float t) => -0.5f * (Mathf.Cos(Mathf.PI * t) - 1f);

        #endregion

        #region Quad

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float InQuad(float t) => t * t;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float OutQuad(float t) => t * (2f - t);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float InOutQuad(float t) => t < 0.5f ? 2f * t * t : 1f - Mathf.Pow(-2f * t + 2f, 2f) * 0.5f;

        #endregion

        #region Cubic

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float InCubic(float t) => t * t * t;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float OutCubic(float t) => 1f - Mathf.Pow(1f - t, 3f);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float InOutCubic(float t) => t < 0.5f ? 4f * t * t * t : 1f - Mathf.Pow(-2f * t + 2f, 3f) * 0.5f;

        #endregion

        #region Quart

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float InQuart(float t) => t * t * t * t;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float OutQuart(float t) => 1f - Mathf.Pow(1f - t, 4f);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float InOutQuart(float t) =>
            t < 0.5f ? 8f * t * t * t * t : 1f - Mathf.Pow(-2f * t + 2f, 4f) * 0.5f;

        #endregion

        #region Quint

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float InQuint(float t) => t * t * t * t * t;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float OutQuint(float t) => 1f - Mathf.Pow(1f - t, 5f);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float InOutQuint(float t) =>
            t < 0.5f ? 16f * t * t * t * t * t : 1f - Mathf.Pow(-2f * t + 2f, 5f) * 0.5f;

        #endregion

        #region Expo

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float InExpo(float t) => t <= 0f ? 0f : Mathf.Pow(2f, 10f * t - 10f);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float OutExpo(float t) => t >= 1f ? 1f : 1f - Mathf.Pow(2f, -10f * t);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float InOutExpo(float t) => t <= 0f ? 0f :
            t >= 1f ? 1f :
            t < 0.5f ? Mathf.Pow(2f, 20f * t - 10f) * 0.5f : (2f - Mathf.Pow(2f, -20f * t + 10f)) * 0.5f;

        #endregion

        #region Circ

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float InCirc(float t) => 1f - Mathf.Sqrt(1f - t * t);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float OutCirc(float t) => Mathf.Sqrt(1f - Mathf.Pow(t - 1f, 2f));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float InOutCirc(float t) => t < 0.5f
            ? (1f - Mathf.Sqrt(1f - 4f * t * t)) * 0.5f
            : (Mathf.Sqrt(1f - Mathf.Pow(-2f * t + 2f, 2f)) + 1f) * 0.5f;

        #endregion

        #region Back

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float InBack(float t) => C3 * t * t * t - C1 * t * t;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float OutBack(float t) => 1f + C3 * Mathf.Pow(t - 1f, 3f) + C1 * Mathf.Pow(t - 1f, 2f);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float InOutBack(float t) => t < 0.5f
            ? (Mathf.Pow(2f * t, 2f) * ((C2 + 1f) * 2f * t - C2)) * 0.5f
            : (Mathf.Pow(2f * t - 2f, 2f) * ((C2 + 1f) * (t * 2f - 2f) + C2) + 2f) * 0.5f;

        #endregion

        #region Elastic

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float InElastic(float t) => t <= 0f ? 0f :
            t >= 1f ? 1f : -Mathf.Pow(2f, 10f * t - 10f) * Mathf.Sin((t * 10f - 10.75f) * C4);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float OutElastic(float t) => t <= 0f ? 0f :
            t >= 1f ? 1f : Mathf.Pow(2f, -10f * t) * Mathf.Sin((t * 10f - 0.75f) * C4) + 1f;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float InOutElastic(float t) => t <= 0f
            ? 0f
            : t >= 1f
                ? 1f
                : t < 0.5f
                    ? -(Mathf.Pow(2f, 20f * t - 10f) * Mathf.Sin((20f * t - 11.125f) * C5)) * 0.5f
                    : (Mathf.Pow(2f, -20f * t + 10f) * Mathf.Sin((20f * t - 11.125f) * C5)) * 0.5f + 1f;

        #endregion

        #region Bounce

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float InBounce(float t) => 1f - OutBounce(1f - t);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float OutBounce(float t)
        {
            if (t < 1f / D1) return N1 * t * t;
            if (t < 2f / D1) return N1 * (t -= 1.5f / D1) * t + 0.75f;
            if (t < 2.5f / D1) return N1 * (t -= 2.25f / D1) * t + 0.9375f;
            return N1 * (t -= 2.625f / D1) * t + 0.984375f;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float InOutBounce(float t) => t < 0.5f
            ? (1f - OutBounce(1f - 2f * t)) * 0.5f
            : (1f + OutBounce(2f * t - 1f)) * 0.5f;

        #endregion
    }
}