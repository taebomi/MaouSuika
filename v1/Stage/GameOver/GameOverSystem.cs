using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using FMODUnity;
using SOSG.System;
using SOSG.System.Audio;
using SOSG.Utility.Countdown;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SOSG.Stage.GameOver
{
    public class GameOverSystem : MonoBehaviour
    {
        [Header("Event SO")] [SerializeField] private StageAdsSO stageAdsSO;
        [SerializeField] private GameOverSystemStateSO stateSO;
        [SerializeField] private GashaponExplodeEffectRequestEventSO gashaponExplodeEffectRequestEventSO;

        [Header("Variable SO")] [SerializeField]
        private GashaponLinkedListVarSO activeGashaponListVarSO;


        [SerializeField] private BoolVariableSO isWarningVarSO;
        [SerializeField] private CountdownTimerSO gameOverCountdownTimerSO;

        [SerializeField] private GameOverUI gameOverUI;

        [Header("Sfx")] [SerializeField] private EventReference gashaponExplodeSfx;

        private void Awake()
        {
            stateSO.Initialize();

            isWarningVarSO.OnValueChanged += OnWarningChanged;
            gameOverCountdownTimerSO.OnStateChanged += OnCountdownStateChanged;
            stateSO.ActionOnStateChanged += OnStateChanged;
        }

        private void OnDestroy()
        {
            AudioSystemHelper.SetBgmPitch(1f);

            gameOverCountdownTimerSO.OnStateChanged -= OnCountdownStateChanged;
            isWarningVarSO.OnValueChanged -= OnWarningChanged;
            stateSO.ActionOnStateChanged -= OnStateChanged;
        }


        public void OnWarningChanged(bool value)
        {
            if (gameOverCountdownTimerSO.State is CountdownState.Started)
            {
                ChangeState(GameOverSystemState.Countdown);
            }
            else
            {
                if (value)
                {
                    ChangeState(GameOverSystemState.Warning);
                }
                else
                {
                    ChangeState(GameOverSystemState.Safe);
                }
            }
        }

        private void OnCountdownStateChanged(CountdownState state)
        {
            switch (state)
            {
                case CountdownState.Canceled:
                    if (isWarningVarSO.Value)
                    {
                        ChangeState(GameOverSystemState.Warning);
                    }
                    else
                    {
                        ChangeState(GameOverSystemState.Safe);
                    }

                    break;
                case CountdownState.Started:
                    ChangeState(GameOverSystemState.Countdown);
                    break;
                case CountdownState.Finished:
                    ChangeState(GameOverSystemState.GameOver);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        private void ChangeState(GameOverSystemState gameOverSystemState)
        {
            if (stateSO.CurState == gameOverSystemState)
            {
                return;
            }

            if (stateSO.CurState is GameOverSystemState.GameOver)
            {
                return;
            }

            stateSO.ChangeState(gameOverSystemState);
        }

        private void OnStateChanged(GameOverSystemState state)
        {
            // switch (state)
            // {
            //     case GameOverSystemState.Safe:
            //         audioEventSO.ChangeBgmPitch(1f, AudioManager.PitchChangeSpeedNormal);
            //         break;
            //     case GameOverSystemState.Warning:
            //         audioEventSO.ChangeBgmPitch(1.25f, AudioManager.PitchChangeSpeedNormal);
            //         break;
            //     case GameOverSystemState.Countdown:
            //         audioEventSO.ChangeBgmPitch(1.5f, AudioManager.PitchChangeSpeedVeryFast);
            //         break;
            //     case GameOverSystemState.GameOver:
            //         audioEventSO.ChangeBgmPitch(1f, AudioManager.PitchChangeSpeedNormal);
            //         GameOver().Forget();
            //         break;
            //     default:
            //         throw new ArgumentOutOfRangeException(nameof(state), state, null);
            // }
        }


        private async UniTaskVoid GameOver()
        {
            await DestroyGashapon();

            stageAdsSO.ShowInterstitialAd();
            gameOverUI.Activate().Forget();
        }

        private async UniTask DestroyGashapon()
        {
            var gashaponList = activeGashaponListVarSO.Value;
            foreach (var gashapon in gashaponList)
            {
                gashapon.ActivatePhysics(false);
            }

            while (gashaponList.Last is not null)
            {
                var gashapon = gashaponList.Last.Value;
                gashaponList.RemoveLast();

                var effect = gashaponExplodeEffectRequestEventSO.Request(gashapon.CurLevel);
                effect.SetColor(gashapon.MonsterDataSO.mergeEffectColor);
                effect.SetSize(gashapon.CurSize);
                effect.transform.position = gashapon.transform.position;

                gashapon.Deactivate();
                AudioSystemHelper.PlaySfx(gashaponExplodeSfx, Random.Range(0.8f, 1.2f));

                await UniTask.Delay(TimeSpan.FromSeconds(Random.Range(0.075f, 0.125f)),
                    cancellationToken: destroyCancellationToken);
            }
        }
    }
}