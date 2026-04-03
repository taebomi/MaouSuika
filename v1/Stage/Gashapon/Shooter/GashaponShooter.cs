using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using QFSW.QC;
using UnityEngine;

namespace SOSG.Stage
{
    public class GashaponShooter : MonoBehaviour
    {
        [Header("Event SO")]
        [SerializeField] private VoidEventSO stageStartEventSO;
        [SerializeField] private VoidEventSO stageEndEventSO;

        [SerializeField] private GashaponRequestEventSO gashaponRequestEventSO;
        [SerializeField] private VoidEventSO shotEventSO;
        [SerializeField] private BoolEventSO collidedStateChangedEventSO;

        [SerializeField] private GashaponShooterControlEventSO shotRequestEventSO;


        [Header("Variable SO")]
        [SerializeField] private GashaponVarSO loadedGashaponVarSO;
        [SerializeField] private GashaponVarSO lastShotGashaponVarSO;

        [SerializeField] private BoolVariableSO isCooldownVarSO;
        [SerializeField] private BoolVariableSO isCollidedVarSO;

        [Header("Component")]
        [SerializeField] private GashaponQueue gashaponQueue;

        private State _lastState;
        private State _curState;

        [Flags]
        public enum State
        {
            None,
            Shootable,
            Cooldown,
            Collided,
        }

        private bool CanShoot => _curState == State.Shootable;


        // 상수
        // 발사 관련
        private const float ShootPower = 20f;
        private const float RotateSpeed = 135f;

        // 발사 조건 관련
        public static readonly Color CollidedColor = Color.red;
        public const float CooldownAlpha = 0.3333f;


        private void Awake()
        {
            _lastState = _curState = State.None;
            loadedGashaponVarSO.value = null;
            lastShotGashaponVarSO.value = null;


            isCollidedVarSO.OnValueChanged += OnCollidedChanged;
            isCooldownVarSO.OnValueChanged += OnCooldownChanged;
            shotRequestEventSO.ActionOnControl += OnShootRequested;
            stageStartEventSO.OnEventRaised += OnStageStarted;
            stageEndEventSO.OnEventRaised += OnStageEnded;
        }

        private void OnDestroy()
        {
            stageStartEventSO.OnEventRaised -= OnStageStarted;
            stageEndEventSO.OnEventRaised -= OnStageEnded;
            isCollidedVarSO.OnValueChanged -= OnCollidedChanged;
            isCooldownVarSO.OnValueChanged -= OnCooldownChanged;
            shotRequestEventSO.ActionOnControl -= OnShootRequested;
        }

        private void Update()
        {
            if (CanShoot && loadedGashaponVarSO.value)
            {
                RotateLoadedGashapon();
            }
        }

        private void OnStageStarted()
        {
            _lastState = _curState = State.Shootable;
            ReloadGashapon();
        }

        private void OnStageEnded()
        {
            loadedGashaponVarSO.value = null;
        }

        private void OnShootRequested(Vector2 dir, float ratio)
        {
            if ((CanShoot && loadedGashaponVarSO.value) is false)
            {
                return;
            }

            ShootGashapon(dir, ratio);
        }

        private void OnCooldownChanged(bool value)
        {
            if (value)
            {
                _curState = State.Cooldown;
            }
            else
            {
                _curState = isCollidedVarSO.Value ? State.Collided : State.Shootable;
            }

            ApplyState();
        }

        private void OnCollidedChanged(bool value)
        {
            if (isCooldownVarSO.Value)
            {
                _curState = State.Cooldown;
            }
            else
            {
                _curState = value ? State.Collided : State.Shootable;
            }

            if (_curState != _lastState)
            {
                ApplyState();
            }
        }

        private void ApplyState()
        {
            if (_curState is State.Collided || _lastState is State.Collided)
            {
                collidedStateChangedEventSO.RaiseEvent(_curState is State.Collided);
            }

            loadedGashaponVarSO.value.SetShooterState(_curState);
            _lastState = _curState;
        }


        private void ShootGashapon(Vector2 dir, float ratio)
        {
            var loadedGashapon = loadedGashaponVarSO.value;
            loadedGashapon.Shoot(dir * (ratio * ShootPower));
            lastShotGashaponVarSO.value = loadedGashapon;
            ReloadGashapon();
            shotEventSO.RaiseEvent();
        }

        private void ReloadGashapon()
        {
            var curGashaponLevel = gashaponQueue.Dequeue();
            var reloadedGashapon = gashaponRequestEventSO.Request(curGashaponLevel, transform.position);
            loadedGashaponVarSO.value = reloadedGashapon;
            reloadedGashapon.SetLoadedState();
        }

        private void RotateLoadedGashapon()
        {
            var loadedGashapon = loadedGashaponVarSO.value;
            loadedGashapon.transform.Rotate(0f, 0f, RotateSpeed * Time.deltaTime);
        }


        #region Debug Menu

        private CancellationTokenSource _createCts;

        [Command("start-create-gashapons")]
        private async UniTaskVoid DebugMenu_StartCreateGashapons()
        {
            _createCts = new CancellationTokenSource();
            while (_createCts.IsCancellationRequested is false)
            {
                var dir = new Vector2(UnityEngine.Random.Range(-1f, 1f), 1f).normalized;
                var ratio = 1f;
                ShootGashapon(dir, ratio);
                await UniTask.Delay(TimeSpan.FromSeconds(0.1), cancellationToken: _createCts.Token);
            }
        }

        [Command("stop-create-gashapons")]
        private void DebugMenu_StopCreateGashapons()
        {
            _createCts?.CancelAndDispose();
        }
        #endregion
    }
}