using System.Collections.Generic;
using UnityEngine;

namespace MaouSuika.Gameplay
{
    public class GameplayCameraController : MonoBehaviour
    {
        [SerializeField] private Camera targetCamera;

        [SerializeField] private float shakeFrequency = 12f;

        private Vector2 _currentShakeOffset;
        private float _defaultZPosition;

        private Vector2 _shakeDirection;
        private float _shakeSampleTimer;

        private readonly List<ActiveShake> _activeShakes = new();

        private void Awake()
        {
            _defaultZPosition = targetCamera.transform.localPosition.z;
            enabled = false;
        }

        public void Setup()
        {
            _activeShakes.Clear();
        }

        private void Update()
        {
            if (TickShakes(Time.unscaledDeltaTime)) return;

            enabled = false;
        }

        public Vector2 ScreenToWorldPoint(Vector2 screenPoint)
        {
            var worldPoint = (Vector2)targetCamera.ScreenToWorldPoint(screenPoint);
            return worldPoint - _currentShakeOffset;
        }

        public void RequestShake(CameraShakeRequest request)
        {
            if (request.Strength <= 0f || request.Duration <= 0f) return;

            if (_activeShakes.Count == 0)
            {
                _shakeSampleTimer = 0f;
            }

            _activeShakes.Add(new ActiveShake(request));
            enabled = true;
        }


        private bool TickShakes(float deltaTime)
        {
            var maxStrength = 0f;

            for (var i = _activeShakes.Count - 1; i >= 0; i--)
            {
                var shake = _activeShakes[i];

                shake.RemainingDuration -= deltaTime;
                if (shake.RemainingDuration <= 0f)
                {
                    _activeShakes.RemoveAt(i);
                    continue;
                }

                var remainingRatio = shake.RemainingDuration / shake.Duration;
                var currentStrength = shake.Strength * remainingRatio;

                maxStrength = Mathf.Max(maxStrength, currentStrength);
                _activeShakes[i] = shake;
            }

            if (_activeShakes.Count == 0)
            {
                _currentShakeOffset = Vector2.zero;
                targetCamera.transform.localPosition = new Vector3(0f, 0f, _defaultZPosition);
                return false;
            }

            _shakeSampleTimer -= deltaTime;

            if (_shakeSampleTimer <= 0f)
            {
                _shakeSampleTimer += 1f / shakeFrequency;
                _shakeDirection = Random.insideUnitCircle.normalized;
            }

            _currentShakeOffset = _shakeDirection * maxStrength;

            targetCamera.transform.localPosition =
                new Vector3(_currentShakeOffset.x, _currentShakeOffset.y, _defaultZPosition);

            return true;
        }


        private struct ActiveShake
        {
            public readonly float Strength;
            public readonly float Duration;
            public float RemainingDuration;

            public ActiveShake(CameraShakeRequest request)
            {
                Strength = request.Strength;
                Duration = RemainingDuration = request.Duration;
            }
        }
    }
}