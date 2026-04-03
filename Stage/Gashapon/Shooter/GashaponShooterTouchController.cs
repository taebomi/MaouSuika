using System.Threading;
using Cysharp.Threading.Tasks;
using SOSG.Stage;
using SOSG.System.Input;
using SOSG.System.Setting;
using UnityEngine;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class GashaponShooterTouchController : MonoBehaviour
{
    [SerializeField] private GameInputSO inputSO;

    [SerializeField] private CameraVarSO gashaponCamVarSO;

    [SerializeField] private VoidEventSO gameOverEventSO;
    [SerializeField] private GashaponShooterControlEventSO aimingEventSO;
    [SerializeField] private GashaponShooterControlEventSO shotEventSO;
    [SerializeField] private BoolEventSO aimingStateChangedEventSO;

    private Vector2 _curPressedPoint;
    private Vector2 _lastPressedPoint;
    private Vector2 _pressedDelta;
    private Vector2 _normalizedDir;
    private float _ratio;

    private bool _isTouchedLastFrame;
    private bool _isDeadZone;

    private int _lastTouchId;
    private int _curTouchId;

    private CancellationTokenSource _controlCts;

    private const float DeadZoneRange = 0.5f;

    private void Awake()
    {
        inputSO.ActionOnStageControlEnabled += OnControlEnabled;
        inputSO.ActionOnStageControlDisabled += OnControlDisabled;
        gameOverEventSO.OnEventRaised += OnGameOver;
    }

    private void OnDestroy()
    {
        inputSO.ActionOnStageControlEnabled -= OnControlEnabled;
        inputSO.ActionOnStageControlDisabled -= OnControlDisabled;
        gameOverEventSO.OnEventRaised -= OnGameOver;
        _controlCts?.CancelAndDispose();
    }

    private void OnControlEnabled()
    {
        _controlCts?.CancelAndDispose();
        _controlCts = new CancellationTokenSource();
        CheckTouch().Forget();
    }

    private void OnControlDisabled()
    {
        _controlCts?.Cancel();
        _isDeadZone = true;
        OnTouchCanceled();
    }

    private void OnGameOver()
    {
        _controlCts?.Cancel();
        _isDeadZone = true;
        OnTouchCanceled();
    }

    private async UniTaskVoid CheckTouch()
    {
        while (_controlCts.IsCancellationRequested is false)
        {
            var isTouchedCurFrame = GetTouch();

            if (isTouchedCurFrame)
            {
                if (!_isTouchedLastFrame)
                {
                    _isTouchedLastFrame = true;
                    OnTouchStarted();
                }
                else
                {
                    OnTouchPerformed();
                }
            }
            else
            {
                if (_isTouchedLastFrame)
                {
                    _isTouchedLastFrame = false;
                    OnTouchCanceled();
                }
            }

            await UniTask.Yield(_controlCts.Token);
        }
    }

    private bool GetTouch()
    {
        foreach (var touch in Touch.activeTouches)
        {
            var viewportPoint = (Vector2)gashaponCamVarSO.value.ScreenToViewportPoint(touch.screenPosition);

            if (!IsInsideViewport(viewportPoint))
            {
                continue;
            }

            var worldPoint = gashaponCamVarSO.value.ScreenToWorldPoint(touch.screenPosition);
            _curPressedPoint = worldPoint;
            _curTouchId = touch.touchId;
            return true;
        }

        return false;
    }

    private static bool IsInsideViewport(Vector2 point)
    {
        return point.x is >= 0f and <= 1f && point.y is >= 0f and <= 1f;
    }

    private void OnTouchStarted()
    {
        _pressedDelta = Vector2.zero;
        _lastPressedPoint = _curPressedPoint;
        _lastTouchId = _curTouchId;
        _isDeadZone = true;
    }

    private void OnTouchPerformed()
    {
        if (_curTouchId == _lastTouchId)
        {
            var delta = _curPressedPoint - _lastPressedPoint;
            var yInvert = SettingDataHelper.ControlSetting.invertYAxis ? -1f : 1f;
            var calibratedDelta = delta * yInvert;
            _pressedDelta += calibratedDelta;

            if (_pressedDelta.y < 0f)
            {
                SetDeadZone();
                _pressedDelta -= calibratedDelta;
                return;
            }

            var dirLength = _pressedDelta.magnitude;
            if (dirLength <= DeadZoneRange)
            {
                SetDeadZone();
                _pressedDelta -= calibratedDelta;
                return;
            }

            _normalizedDir = _pressedDelta / dirLength;
            var maxRange = SettingDataHelper.ControlSetting.dragRange;
            if (dirLength > maxRange)
            {
                dirLength = maxRange;
            }

            _ratio = dirLength / maxRange;

            CancelDeadZone();
            aimingEventSO.RaiseEvent(_normalizedDir, _ratio);
        }

        _lastPressedPoint = _curPressedPoint;
        _lastTouchId = _curTouchId;
    }

    private void OnTouchCanceled()
    {
        if (!_isDeadZone)
        {
            shotEventSO.RaiseEvent(_normalizedDir, _ratio);
        }

        aimingStateChangedEventSO.RaiseEvent(false);
    }

    private void SetDeadZone()
    {
        if (_isDeadZone)
        {
            return;
        }

        _isDeadZone = true;
        aimingStateChangedEventSO.RaiseEvent(false);
    }

    private void CancelDeadZone()
    {
        if (!_isDeadZone)
        {
            return;
        }

        _isDeadZone = false;
        aimingStateChangedEventSO.RaiseEvent(true);
    }
}