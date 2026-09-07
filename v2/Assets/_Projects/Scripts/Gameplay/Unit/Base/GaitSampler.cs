using UnityEngine;

namespace MaouSuika.Gameplay
{
    public class GaitSampler
    {
        private static readonly AnimationCurve Uniform =
            AnimationCurve.Linear(0f, 0f, 1f, 1f);

        private AnimationCurve _curve;
        private float _cycleDuration;
        private float _previousProgress;

        public void Setup(AnimationCurve curve, float cycleDuration)
        {
            _curve = curve is { length: > 0 } ? curve : Uniform;
            _cycleDuration = cycleDuration;
            Restart();
        }

        public void Restart()
        {
            _previousProgress = _curve.Evaluate(0f);
        }

        public float StepTime(float normalizedTime)
        {
            var progress = _curve.Evaluate(normalizedTime);
            var delta = progress - _previousProgress;

            if (delta < 0f)
                delta += 1f;

            _previousProgress = progress;

            return delta * _cycleDuration;
        }
    }
}