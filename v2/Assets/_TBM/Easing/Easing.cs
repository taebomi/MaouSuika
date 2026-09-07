using System;

namespace TBM.Core
{
    public static class Easing
    {
        private const float PI = MathF.PI;
        private const float HALF_PI = MathF.PI * 0.5f;

        // --- Sine ---
        public static float InSine(float t) => 1f - MathF.Cos(t * HALF_PI);
        public static float OutSine(float t) => MathF.Sin(t * HALF_PI);
        public static float InOutSine(float t) => -(MathF.Cos(PI * t) - 1f) * 0.5f;

        // --- Quad ---
        public static float InQuad(float t) => t * t;
        public static float OutQuad(float t) => t * (2f - t);

        public static float InOutQuad(float t)
            => t < 0.5f
                ? 2f * t * t
                : 1f - Sq(-2f * t + 2f) * 0.5f;

        // --- Cubic ---
        public static float InCubic(float t) => t * t * t;

        public static float OutCubic(float t)
        {
            var u = 1f - t;
            return 1f - u * u * u;
        }

        public static float InOutCubic(float t)
            => t < 0.5f
                ? 4f * t * t * t
                : 1f - Cube(-2f * t + 2f) * 0.5f;

        // --- Quart ---
        public static float InQuart(float t)
        {
            var s = t * t;
            return s * s;
        }

        public static float OutQuart(float t)
        {
            var u = 1f - t;
            var s = u * u;
            return 1f - s * s;
        }

        public static float InOutQuart(float t)
        {
            if (t < 0.5f)
            {
                var s = t * t;
                return 8f * s * s;
            }

            var u = -2f * t + 2f;
            var v = u * u;
            return 1f - v * v * 0.5f;
        }

        // --- Quint ---
        public static float InQuint(float t)
        {
            var s = t * t;
            return s * s * t;
        }

        public static float OutQuint(float t)
        {
            var u = 1f - t;
            var s = u * u;
            return 1f - s * s * u;
        }

        public static float InOutQuint(float t)
        {
            if (t < 0.5f)
            {
                var s1 = t * t;
                return 16f * s1 * s1 * t;
            }

            var u = -2f * t + 2f;
            var s = u * u;
            return 1f - s * s * u * 0.5f;
        }

        // --- Expo ---
        public static float InExpo(float t)
            => t <= 0f ? 0f : MathF.Pow(2f, 10f * t - 10f);

        public static float OutExpo(float t)
            => t >= 1f ? 1f : 1f - MathF.Pow(2f, -10f * t);

        public static float InOutExpo(float t)
        {
            if (t <= 0f) return 0f;
            if (t >= 1f) return 1f;
            return t < 0.5f
                ? MathF.Pow(2f, 20f * t - 10f) * 0.5f
                : (2f - MathF.Pow(2f, -20f * t + 10f)) * 0.5f;
        }

        // --- Circ ---
        public static float InCirc(float t) => 1f - MathF.Sqrt(1f - t * t);

        public static float OutCirc(float t)
        {
            var u = t - 1f;
            return MathF.Sqrt(1f - u * u);
        }

        public static float InOutCirc(float t)
            => t < 0.5f
                ? (1f - MathF.Sqrt(1f - Sq(2f * t))) * 0.5f
                : (MathF.Sqrt(1f - Sq(-2f * t + 2f)) + 1f) * 0.5f;

        // --- Back ---
        const float BACK_C1 = 1.70158f;
        const float BACK_C2 = BACK_C1 * 1.525f;
        const float BACK_C3 = BACK_C1 + 1f;

        public static float InBack(float t)
            => BACK_C3 * t * t * t - BACK_C1 * t * t;

        public static float OutBack(float t)
        {
            var u = t - 1f;
            return 1f + BACK_C3 * u * u * u + BACK_C1 * u * u;
        }

        public static float InOutBack(float t)
            => t < 0.5f
                ? Sq(2f * t) * ((BACK_C2 + 1f) * 2f * t - BACK_C2) * 0.5f
                : (Sq(2f * t - 2f) * ((BACK_C2 + 1f) * (2f * t - 2f) + BACK_C2) + 2f) * 0.5f;

        // --- Elastic ---
        const float ELASTIC_C4 = 2f * PI / 3f;
        const float ELASTIC_C5 = 2f * PI / 4.5f;

        public static float InElastic(float t)
        {
            if (t <= 0f) return 0f;
            if (t >= 1f) return 1f;
            return -MathF.Pow(2f, 10f * t - 10f) * MathF.Sin((10f * t - 10.75f) * ELASTIC_C4);
        }

        public static float OutElastic(float t)
        {
            if (t <= 0f) return 0f;
            if (t >= 1f) return 1f;
            return MathF.Pow(2f, -10f * t) * MathF.Sin((10f * t - 0.75f) * ELASTIC_C4) + 1f;
        }

        public static float InOutElastic(float t)
        {
            if (t <= 0f) return 0f;
            if (t >= 1f) return 1f;
            return t < 0.5f
                ? -(MathF.Pow(2f, 20f * t - 10f) * MathF.Sin((20f * t - 11.125f) * ELASTIC_C5)) * 0.5f
                : MathF.Pow(2f, -20f * t + 10f) * MathF.Sin((20f * t - 11.125f) * ELASTIC_C5) * 0.5f + 1f;
        }

        // --- Bounce ---
        const float BOUNCE_N1 = 7.5625f;
        const float BOUNCE_D1 = 2.75f;

        public static float OutBounce(float t)
        {
            if (t < 1f / BOUNCE_D1)
                return BOUNCE_N1 * t * t;
            if (t < 2f / BOUNCE_D1)
            {
                t -= 1.5f / BOUNCE_D1;
                return BOUNCE_N1 * t * t + 0.75f;
            }

            if (t < 2.5f / BOUNCE_D1)
            {
                t -= 2.25f / BOUNCE_D1;
                return BOUNCE_N1 * t * t + 0.9375f;
            }

            t -= 2.625f / BOUNCE_D1;
            return BOUNCE_N1 * t * t + 0.984375f;
        }

        public static float InBounce(float t) => 1f - OutBounce(1f - t);

        public static float InOutBounce(float t)
            => t < 0.5f
                ? (1f - OutBounce(1f - 2f * t)) * 0.5f
                : (1f + OutBounce(2f * t - 1f)) * 0.5f;

        static float Sq(float x) => x * x;
        static float Cube(float x) => x * x * x;
    }
}