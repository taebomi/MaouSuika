using System.Collections;
using SOSG.Utility.Countdown;
using UnityEngine;

namespace SOSG.Stage.GameOver
{
    public class CountdownSystem : MonoBehaviour
    {
        [Header("Event SO")]
        [SerializeField] private StageAdsSO stageAdsSO;
        [SerializeField] private BoolEventSO shooterCollidedStateChangedEventSO;
        [SerializeField] private CountdownTimerSO countdownTimerSO;
        
        [Header("Variable SO")]
        [SerializeField] private GashaponVarSO collidedGashaponVarSO;

        private float _timer;

        private IEnumerator _checkingCoroutine;

        private const float CountdownDuration = 5f;

        private void Awake()
        {
            countdownTimerSO.Initialize();

            stageAdsSO.OnRewarded += RestartCountdown;
            shooterCollidedStateChangedEventSO.OnEventRaised += OnShooterCollidedChanged;
        }
        
        private void OnDestroy()
        {
            stageAdsSO.OnRewarded -= RestartCountdown;
            shooterCollidedStateChangedEventSO.OnEventRaised -= OnShooterCollidedChanged;
        }

        private void OnShooterCollidedChanged(bool value)
        {
            if (value)
            {
                _checkingCoroutine = CheckCollideMoving();
                StartCoroutine(_checkingCoroutine);
            }
            else
            {
                StopCoroutine(_checkingCoroutine);
                if (countdownTimerSO.State is CountdownState.Started)
                {
                    countdownTimerSO.ChangeState(CountdownState.Canceled);
                }
            }
        }

        private IEnumerator CheckCollideMoving()
        {
            _timer = CountdownDuration;
            while (_timer > 0f)
            {
                var collidedGashapon = collidedGashaponVarSO.value;
                if (collidedGashapon)
                {
                    if (IsMoving(collidedGashapon))
                    {
                        CancelCountdown();
                    }
                    else
                    {
                        Countdown();
                    }
                }
                else
                {
                    yield break;
                }
                yield return null;
            }
            
            FinishCountdown();
        }

        private void CancelCountdown()
        {
            if (countdownTimerSO.State is not CountdownState.Canceled)
            {
                countdownTimerSO.ChangeState(CountdownState.Canceled);
            }
            
            _timer = CountdownDuration;
        }

        private void RestartCountdown()
        {
            _timer = CountdownDuration;
        }

        private void Countdown()
        {
            if (countdownTimerSO.State is not CountdownState.Started)
            {
                countdownTimerSO.ChangeState(CountdownState.Started);
            }

            _timer -= Time.deltaTime;
        }

        private void FinishCountdown()
        {
            countdownTimerSO.ChangeState(CountdownState.Finished);
        }

        private static bool IsMoving(Gashapon gashapon)
        {
            const float threshold = 0.25f;
            const float sqrThreshold = threshold * threshold;
            var sqrVelocityMag = gashapon.Rb.linearVelocity.sqrMagnitude;
            return sqrVelocityMag > sqrThreshold;
        }
    }
}