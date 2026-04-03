using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using SOSG.Stage.Suika;
using SOSG.Stage.SplitScreenMode;
using SOSG.Stage.Suika.GameOver;
using SOSG.System.Audio;
using UnityEngine;
using UnityEngine.Events;

namespace SOSG.Stage.GameOver
{
    public class DangerStateBgmController : MonoBehaviour
    {
        private List<DangerStateMonitor> _dangerStateMonitorList;

        private BgmPitchMode _curPitchMode;
        private BgmController _bgmController;

        private CancellationTokenSource _bgmCts;

        private enum BgmPitchMode
        {
            Idle,
            Warning,
            Countdown,
        }

        private void Awake()
        {
            _bgmCts = new CancellationTokenSource();
            _bgmController = new BgmController(_bgmCts);
        }


        private void OnDisable()
        {
            _bgmCts.Cancel();
        }

        private void OnDestroy()
        {
            _bgmCts.Dispose();
        }

        public void SetUp(PlayerSuikaManager[] playerSuikaArr)
        {
            _dangerStateMonitorList = new List<DangerStateMonitor>();
            foreach (var playerSuika in playerSuikaArr)
            {
                _dangerStateMonitorList.Add(playerSuika.GameOverSystem.DangerStateMonitor);
            }

            foreach (var dangerStateContainer in _dangerStateMonitorList)
            {
                dangerStateContainer.DangerStateChanged += CheckState;
            }
        }

        public void TearDown()
        {
            foreach (var dangerStateMonitor in _dangerStateMonitorList)
            {
                dangerStateMonitor.DangerStateChanged -= CheckState;
            }
        }

        private void CheckState()
        {
            foreach (var dangerStateContainer in _dangerStateMonitorList)
            {
                switch (dangerStateContainer.DangerState)
                {
                    case DangerState.Countdown:
                        ChangeState(BgmPitchMode.Countdown);
                        return;
                    case DangerState.Warning:
                        ChangeState(BgmPitchMode.Warning);
                        return;
                    case DangerState.None:
                        continue;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            ChangeState(BgmPitchMode.Idle);
        }

        private void ChangeState(BgmPitchMode pitchMode)
        {
            if (_curPitchMode == pitchMode)
            {
                return;
            }

            _bgmController.SetPitch(pitchMode);
            _curPitchMode = pitchMode;
        }

        private class BgmController
        {
            private float _curPitch;
            private float _destPitch;
            private bool _isTransitioning;

            private readonly CancellationTokenSource _cts;

            public BgmController(CancellationTokenSource cts)
            {
                _curPitch = 1f;
                _destPitch = 1f;
                _cts = cts;
            }

            public void SetPitch(BgmPitchMode mode)
            {
                _destPitch = mode switch
                {
                    BgmPitchMode.Idle => 1f,
                    BgmPitchMode.Warning => 1.25f,
                    BgmPitchMode.Countdown => 1.5f,
                    _ => throw new ArgumentOutOfRangeException()
                };

                if (_isTransitioning)
                {
                    return;
                }

                ChangeBgmPitchAsync().Forget();
            }

            private async UniTaskVoid ChangeBgmPitchAsync()
            {
                _isTransitioning = true;
                while (Mathf.Approximately(_curPitch, _destPitch) is false &&
                       _cts.IsCancellationRequested is false)
                {
                    _curPitch = Mathf.MoveTowards(_curPitch, _destPitch, 0.5f * Time.deltaTime);
                    AudioSystemHelper.SetBgmPitch(_curPitch);
                    await UniTask.Yield(_cts.Token);
                }

                _curPitch = _destPitch;
                AudioSystemHelper.SetBgmPitch(_curPitch);
                _isTransitioning = false;
            }
        }
    }
}