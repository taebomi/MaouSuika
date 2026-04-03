using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace SOSG.Stage.GameOver
{
    public class RedScreen : MonoBehaviour
    {
        [Header("Event SO - Listener")]
        [SerializeField] private GameOverSystemStateSO gameOverSystemStateSO;

        [Header("Component")]
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Image redScreenImage;

        private float _curAlpha;
        private float _timer;
        private float _speedMultiplier;

        private bool _isActivated;

        private CancellationTokenSource _fadingCts;

        private const float SpeedMultiplierNormal = 4f;
        private const float SpeedMultiplierFast = 8f;

        private const float ActivationSpeed = 2f;

        private void Awake()
        {
            canvasGroup.alpha = 0f;
            _speedMultiplier = 1f;
            _isActivated = false;

            gameOverSystemStateSO.ActionOnStateChanged += OnGameOverSystemStateChanged;
        }

        private void OnDestroy()
        {
            gameOverSystemStateSO.ActionOnStateChanged -= OnGameOverSystemStateChanged;

            _fadingCts?.CancelAndDispose();
        }

        private void OnGameOverSystemStateChanged(GameOverSystemState gameOverSystemState)
        {
            switch (gameOverSystemState)
            {
                case GameOverSystemState.Safe:
                    Deactivate();
                    break;
                case GameOverSystemState.Warning:
                    _speedMultiplier = SpeedMultiplierNormal;
                    Activate();
                    break;
                case GameOverSystemState.Countdown:
                    _speedMultiplier = SpeedMultiplierFast;
                    Activate();
                    break;
                case GameOverSystemState.GameOver:
                    Deactivate();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(gameOverSystemState), gameOverSystemState, null);
            }
        }

        private void Activate()
        {
            if (_isActivated)
            {
                return;
            }

            _isActivated = true;
            _fadingCts?.CancelAndDispose();
            _fadingCts = new CancellationTokenSource();
            ShowCanvasGroup(_fadingCts.Token).Forget();
            RepeatRedScreenAlpha(_fadingCts.Token).Forget();
        }

        private void Deactivate()
        {
            if (!_isActivated)
            {
                return;
            }

            _isActivated = false;
            _fadingCts?.CancelAndDispose();
            _fadingCts = new CancellationTokenSource();
            HideCanvasGroup(_fadingCts.Token).Forget();
        }

        private async UniTaskVoid ShowCanvasGroup(CancellationToken ct)
        {
            while (canvasGroup.alpha < 1f && ct.IsCancellationRequested is false)
            {
                canvasGroup.alpha += ActivationSpeed * Time.deltaTime;
                await UniTask.Yield(ct);
            }

            canvasGroup.alpha = 1f;
        }
        
        private async UniTaskVoid HideCanvasGroup(CancellationToken ct)
        {
            while (canvasGroup.alpha > 0f && ct.IsCancellationRequested is false)
            {
                canvasGroup.alpha -= ActivationSpeed * Time.deltaTime;
                await UniTask.Yield(ct);
            }

            canvasGroup.alpha = 0f;
        }

        private async UniTaskVoid RepeatRedScreenAlpha(CancellationToken ct)
        {
            while(_isActivated && ct.IsCancellationRequested is false)
            {
                var alpha = Mathf.Sin(_timer) * 0.25f + 0.75f; // 0.5f ~ 1f의 값을 가짐.
                redScreenImage.color = new Color(1f, 1f, 1f, alpha);
                _timer += Time.deltaTime * _speedMultiplier;
                await UniTask.Yield(ct);
            }
        }
    }
}