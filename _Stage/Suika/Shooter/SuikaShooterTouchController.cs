using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using SOSG.System.Setting;
using TaeBoMi;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace SOSG.Stage.Suika.Shooter
{
    public class SuikaShooterTouchController : MonoBehaviour
    {
        private const float DeadZoneLength = 0.5f;

        [SerializeField] private SuikaShooter shooter;
        [SerializeField] private SuikaShooterAim aim;

        [SerializeField] private Camera cam;

        private LinkedList<Finger> _fingerList;

        private Vector2 _delta;
        private Vector2 _normalizedDir;
        private float _powerRatio;

        private bool _isDeadZone;
        private bool _screenYInverted;

        private float _deadZoneScreenSpaceLength;

        private CancellationTokenSource _enableCts;

        private void Awake()
        {
            _fingerList = new LinkedList<Finger>();
            _deadZoneScreenSpaceLength = WorldToScreenDist(DeadZoneLength);
        }

        private void OnEnable()
        {
            _enableCts?.Dispose();
            _enableCts = new CancellationTokenSource();
        }

        private void OnDisable()
        {
            _enableCts.Cancel();
            _fingerList.Clear();
        }

        public void SetScreenYInverted(bool value)
        {
            _screenYInverted = value;
        }

        public void AddFinger(Finger finger)
        {
            if (isActiveAndEnabled is false)
            {
                return;
            }

            _fingerList.AddFirst(finger);
            if (_fingerList.Count == 1)
            {
                CheckTouch(_enableCts.Token).Forget();
            }
        }

        public void RemoveFinger(Finger finger)
        {
            if (isActiveAndEnabled is false)
            {
                return;
            }

            _fingerList.Remove(finger);
            if (_fingerList.Count == 0)
            {
                OnTouchCanceled();
            }
        }

        private void OnTouchStarted(Finger finger)
        {
        }

        private async UniTaskVoid CheckTouch(CancellationToken ct)
        {
            _delta = new Vector2();
            var maxRange = WorldToScreenDist(SettingDataHelper.ControlSetting.dragRange);
            var lastTouch = _fingerList.First.Value.currentTouch;
            var lastPos = lastTouch.screenPosition;
            while (_fingerList.Count != 0 && ct.IsCancellationRequested is false)
            {
                var finger = _fingerList.First.Value;
                var curPos = finger.currentTouch.screenPosition;
                Vector2 delta;
                if (lastTouch.finger == finger)
                {
                    delta = curPos - lastPos;
                }
                else
                {
                    delta = Vector2.zero;
                }

                lastTouch = finger.currentTouch;
                lastPos = curPos;

                var yInvert = SettingDataHelper.ControlSetting.invertYAxis ? -1f : 1f;
                yInvert = _screenYInverted ? -yInvert : yInvert;
                var calibratedDelta = delta * yInvert;
                _delta += calibratedDelta;
                var deltaLength = _delta.magnitude;
                _normalizedDir = _delta / deltaLength;

                if (deltaLength > maxRange)
                {
                    deltaLength = maxRange;
                    _delta = _normalizedDir * deltaLength;
                }

                if (_delta.y < 0f)
                {
                    SetDeadZone();
                    await UniTask.Yield(ct);
                    continue;
                }

                if (deltaLength <= _deadZoneScreenSpaceLength)
                {
                    SetDeadZone();
                    await UniTask.Yield(ct);
                    continue;
                }

                UnsetDeadZone();

                _powerRatio = deltaLength / maxRange;
                aim.UpdateDirection(_normalizedDir, _powerRatio);
                await UniTask.Yield(ct);
            }
        }

        private float WorldToScreenDist(float worldDist)
        {
            var worldSpacePoint = cam.WorldToScreenPoint(new Vector3(worldDist, 0f));
            var originSpacePoint = cam.WorldToScreenPoint(Vector3.zero);
            return (worldSpacePoint - originSpacePoint).magnitude;
        }

        private void OnTouchCanceled()
        {
            aim.SetActive(false);
            if (_isDeadZone is false)
            {
                shooter.ShootSuika(_normalizedDir, _powerRatio);
            }
        }

        private void SetDeadZone()
        {
            if (_isDeadZone)
            {
                return;
            }

            _isDeadZone = true;
            aim.SetActive(false);
        }

        private void UnsetDeadZone()
        {
            if (_isDeadZone is false)
            {
                return;
            }

            _isDeadZone = false;
            aim.SetActive(true);
        }
    }
}