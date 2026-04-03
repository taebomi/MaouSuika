using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using SOSG.Stage.GameOver;
using UnityEngine;
using UnityEngine.UI;

namespace SOSG.Stage.Suika.GameOver
{
    public class ScreenDangerEffect : MonoBehaviour
    {
        [SerializeField] private DangerStateMonitor stateMonitor;
        [SerializeField] private RawImage image;

        private ShowingController _showingController;
        private MaterialController _materialController;

        private DangerState _curState;

        public void Awake()
        {
            _showingController = new ShowingController(image);
            _materialController = new MaterialController(image);
        }

        private void OnEnable()
        {
            stateMonitor.DangerStateChanged += OnDangerStateChanged;
        }

        private void OnDisable()
        {
            stateMonitor.DangerStateChanged -= OnDangerStateChanged;
        }

        private void OnDangerStateChanged()
        {
            var state = stateMonitor.DangerState;
            switch (state)
            {
                case DangerState.None:
                    _showingController.Hide();
                    break;
                case DangerState.Warning:
                    _showingController.Show();
                    _materialController.SetWarning();
                    break;
                case DangerState.Countdown:
                    _showingController.Show();
                    _materialController.SetCountdown();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }


        private class ShowingController
        {
            private const float FadingSpeed = 2f;

            private readonly RawImage _image;
            private float _curAlpha;
            private float _destAlpha;

            private bool _isTransitioning;

            public ShowingController(RawImage image)
            {
                _image = image;
                _curAlpha = 0f;
            }

            public void Show()
            {
                SetTarget(1f);
            }

            public void Hide()
            {
                SetTarget(0f);
            }

            private void SetTarget(float destAlpha)
            {
                _destAlpha = destAlpha;
                if (_isTransitioning)
                {
                    return;
                }

                FadeAsync().Forget();
            }

            private async UniTaskVoid FadeAsync()
            {
                _isTransitioning = true;
                _image.gameObject.SetActive(true);

                while (Mathf.Approximately(_curAlpha, _destAlpha) is false &&
                       _image.destroyCancellationToken.IsCancellationRequested is false)
                {
                    _curAlpha = Mathf.MoveTowards(_curAlpha, _destAlpha, Time.deltaTime * FadingSpeed);
                    _image.color = new Color(1f, 1f, 1f, _curAlpha);
                    await UniTask.Yield(_image.destroyCancellationToken);
                }

                _curAlpha = _destAlpha;
                _image.color = new Color(1f, 1f, 1f, _curAlpha);
                _isTransitioning = false;
                if (Mathf.Approximately(_curAlpha, 0f))
                {
                    _image.gameObject.SetActive(false);
                }
            }
        }

        private class MaterialController
        {
            private const float WarningSize = 3f;
            private const float CountdownSize = 2f;
            private const float ChangingSpeed = 2f;

            private readonly RawImage _image;
            private readonly CancellationTokenSource _cts;

            private float _curSize;
            private float _destSize;

            private bool _isTransitioning;

            private static int sizePropertyCache;

            [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
            private static void InitializeOnSubsystemRegistration()
            {
                sizePropertyCache = Shader.PropertyToID("_Size");
            }

            public MaterialController(RawImage image)
            {
                _image = image;
                _curSize = WarningSize;
            }

            public void SetWarning()
            {
                SetSize(WarningSize);
            }

            public void SetCountdown()
            {
                SetSize(CountdownSize);
            }

            private void SetSize(float size)
            {
                _destSize = size;
                if (_isTransitioning)
                {
                    return;
                }

                ChangeSize().Forget();
            }

            private async UniTaskVoid ChangeSize()
            {
                _isTransitioning = true;

                while (Mathf.Approximately(_curSize, _destSize) is false &&
                       _image.destroyCancellationToken.IsCancellationRequested is false)
                {
                    _curSize = Mathf.MoveTowards(_curSize, _destSize, Time.deltaTime * ChangingSpeed);
                    _image.material.SetFloat(sizePropertyCache, _curSize);
                    await UniTask.Yield(_image.destroyCancellationToken);
                }

                _curSize = _destSize;
                _image.material.SetFloat(sizePropertyCache, _curSize);
                _isTransitioning = false;
            }
        }
    }
}