using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Febucci.UI;
using Febucci.UI.Core.Parsing;
using FMODUnity;
using SOSG.System.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace SOSG.System.Dialogue
{
    public class ConversationTextBox : MonoBehaviour
    {
        public enum State
        {
            Idle,
            Printing,
            Waiting,
            Disappearing,
        }

        private const float WaitingDuration = 3f;
        private const int LineLimit = 4;
        private const int ScrollStartLineIdx = LineLimit - 1;
        private const float DefaultWaitLong = 0.25f;
        private const float ScrollWaitDuration = 1f;

        [SerializeField] private ConversationController controller;

        [SerializeField] private TypewriterByCharacter typewriter;
        [SerializeField] private TextAnimator_TMP textAnimator;
        [SerializeField] private TMP_Text tmp;

        public State CurState { get; private set; } = State.Idle;

        private bool _isAutoScrolling;
        private bool _isPaused;

        private CancellationTokenSource _autoScrollCts = new();
        private CancellationTokenSource _waitingCts = new();

        private void OnEnable()
        {
            DialogueHelper.ConversationPauseRequested += Pause;
            DialogueHelper.ConversationUnPauseRequested += UnPause;
        }
        
        private void OnDisable()
        {
            DialogueHelper.ConversationPauseRequested -= Pause;
            DialogueHelper.ConversationUnPauseRequested -= UnPause;
            
            StopAutoScroll();
            if (CurState is State.Waiting)
            {
                _waitingCts.Cancel();
            }
        }

        public void Print(string text)
        {
            CurState = State.Printing;
            typewriter.ShowText(text);
            typewriter.StartShowingText();

            if (tmp.textInfo.lineCount > LineLimit)
            {
                StartAutoScroll();
            }
        }

        #region Pause

        public void Pause()
        {
            if (_isPaused)
            {
                return;
            }
            
            _isPaused = true;
            if (CurState is State.Printing)
            {
                typewriter.StopShowingText();
            }
            else if (CurState is State.Disappearing)
            {
                typewriter.StopDisappearingText();
            }

        }

        public void UnPause()
        {
            if (_isPaused is false)
            {
                return;
            }

            _isPaused = false;
            
            if (CurState is State.Printing)
            {
                typewriter.StartShowingText();
            }
            else if (CurState is State.Disappearing)
            {
                typewriter.StartDisappearingText();
            }
        }

        #endregion


        public void ResetTextBox()
        {
            if (CurState is State.Waiting)
            {
                _waitingCts.Cancel();
            }
            else if (CurState is State.Disappearing)
            {
                typewriter.StopDisappearingText();
            }

            tmp.rectTransform.anchoredPosition = Vector2.zero;
            CurState = State.Idle;
        }


        public void Skip()
        {
            if (CurState is State.Printing)
            {
                typewriter.SkipTypewriter();
            }
            else if (CurState is State.Waiting)
            {
                _waitingCts.Cancel();
                StartDisappear();
            }
            else if (CurState is State.Disappearing)
            {
                typewriter.SkipTypewriter();
            }
        }

        public async UniTaskVoid StartWaiting()
        {
            CurState = State.Waiting;
            if (_waitingCts.IsCancellationRequested)
            {
                _waitingCts.Dispose();
                _waitingCts = new CancellationTokenSource();
            }

            var timer = 0f;
            while (timer < WaitingDuration)
            {
                if (_isPaused is false)
                {
                    timer += Time.unscaledDeltaTime;
                }

                await UniTask.Yield(_waitingCts.Token);
            }

            controller.OnConversationPrinted();
            StartDisappear();
        }

        public void StartDisappear()
        {
            CurState = State.Disappearing;
            typewriter.StartDisappearingText();
        }

        public void OnFinished()
        {
            CurState = State.Idle;
        }

        #region Auto Scroll

        private void StartAutoScroll()
        {
            if (_isAutoScrolling)
            {
                _autoScrollCts.Cancel();
            }

            _isAutoScrolling = true;
            if (_autoScrollCts.IsCancellationRequested)
            {
                _autoScrollCts.Dispose();
                _autoScrollCts = new CancellationTokenSource();
            }

            AutoScrollAsync(_autoScrollCts.Token).Forget();
        }

        private void StopAutoScroll()
        {
            if (_isAutoScrolling is false)
            {
                return;
            }

            _autoScrollCts.Cancel();
            _isAutoScrolling = false;
        }

        private async UniTaskVoid AutoScrollAsync(CancellationToken ct)
        {
            var lineCount = tmp.textInfo.lineCount;
            var lineInfo = tmp.textInfo.lineInfo;
            for (var lineIdx = ScrollStartLineIdx; lineIdx < lineCount - 1; lineIdx++)
            {
                var curLineInfo = lineInfo[lineIdx];
                await CheckScroll(curLineInfo, ct);
            }
        }

        private async UniTask CheckScroll(TMP_LineInfo curLineInfo, CancellationToken ct)
        {
            while (ct.IsCancellationRequested is false)
            {
                if (IsCharacterVisible(curLineInfo.lastCharacterIndex) is false)
                {
                    // 일정시간 대기시킬 경우
                    // typeWriter.StopShowingText();
                    // await UniTask.Delay(TimeSpan.FromSeconds(ScrollWaitDuration), ignoreTimeScale: true,
                    //     cancellationToken: ct);
                    tmp.rectTransform.anchoredPosition += new Vector2(0f, curLineInfo.lineHeight);
                    // typeWriter.StartShowingText();
                    return;
                }

                await UniTask.Yield(PlayerLoopTiming.PostLateUpdate, ct);
            }

            return;

            bool IsCharacterVisible(int charIdx)
            {
                return textAnimator.Characters[charIdx].isVisible;
            }
        }

        #endregion
    }
}