using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace SOSG.Stage.Suika.Score
{
    public class CurrentScoreUI : MonoBehaviour
    {
        private const float AccumulatedScoreYOffset = 40f;

        [SerializeField] private AccumulatedScoreUIElementPoolSO poolSO;

        [SerializeField] private Transform accumulatedScoreContainer;
        [SerializeField] private TMP_Text curScoreTmp;

        private AccumulatedScoreUIElement _curAccumulatedScore;
        private LinkedList<AccumulatedScoreUIElement> _accumulatedScoreList;

        private int _curScore;
        private bool _isAddingToCurScore;

        private void Awake()
        {
            _accumulatedScoreList = new LinkedList<AccumulatedScoreUIElement>();
            _curScore = 0;
            _isAddingToCurScore = false;
        }


        public void AddScore(int score)
        {
            if (_curAccumulatedScore is null)
            {
                _curAccumulatedScore = Create();
            }

            _curAccumulatedScore.AddScore(score);
        }

        public void StopAccumulating()
        {
            if (_curAccumulatedScore is null)
            {
                return;
            }

            _curAccumulatedScore.IsAccumulating = false;
            _curAccumulatedScore = null;

            if (_isAddingToCurScore)
            {
                return;
            }

            AddAccumulatedScoreToCurScore().Forget();
        }

        private AccumulatedScoreUIElement Create()
        {
            var accumulatedScore = poolSO.Get(accumulatedScoreContainer);
            if (_accumulatedScoreList.Count > 0)
            {
                var yPos = _accumulatedScoreList.Last.Value.CurYPos - AccumulatedScoreYOffset;
                accumulatedScore.SetUp(new Vector2(0f, yPos));
            }
            else
            {
                accumulatedScore.SetUp(Vector2.zero);
            }

            _accumulatedScoreList.AddLast(accumulatedScore);
            return accumulatedScore;
        }

        private async UniTaskVoid AddAccumulatedScoreToCurScore()
        {
            _isAddingToCurScore = true;
            while (_accumulatedScoreList.Count > 0)
            {
                var firstAccumulatedScore = _accumulatedScoreList.First.Value;
                if (firstAccumulatedScore.IsAccumulating)
                {
                    break;
                }

                await AddAccumulatedScoreToCurScore(firstAccumulatedScore);
                firstAccumulatedScore.MakeDisappear();
                await ReplaceAccumulatedScores();
                _accumulatedScoreList.RemoveFirst();
                firstAccumulatedScore.Return();
            }

            _isAddingToCurScore = false;
        }

        private async UniTask AddAccumulatedScoreToCurScore(AccumulatedScoreUIElement accumulatedScore)
        {
            var remainedScore = accumulatedScore.Score;
            var timer = 0f;
            while (remainedScore > 0 && destroyCancellationToken.IsCancellationRequested is false)
            {
                const int defaultIncreaseSpeed = 125;

                var sec = (int)timer;
                var speed = defaultIncreaseSpeed << sec;
                var addScore = (int)(speed * Time.deltaTime);
                if (remainedScore < addScore)
                {
                    break;
                }

                remainedScore -= addScore;
                _curScore += addScore;
                curScoreTmp.text = $"{_curScore:000000}";
                accumulatedScore.UpdateText(remainedScore);

                timer += Time.deltaTime;
                await UniTask.Yield(destroyCancellationToken);
            }

            _curScore += remainedScore;
            curScoreTmp.text = $"{_curScore:000000}";
            accumulatedScore.UpdateText(0);
        }

        private async UniTask ReplaceAccumulatedScores()
        {
            var timer = 0f;
            const float duration = 0.5f;
            int idx;
            while (timer < duration && destroyCancellationToken.IsCancellationRequested is false)
            {
                var yPos = AccumulatedScoreYOffset * Easing.InCubic(timer, duration);
                idx = 0;
                foreach (var element in _accumulatedScoreList)
                {
                    element.SetYPosition(idx * -AccumulatedScoreYOffset + yPos);
                    idx++;
                }

                timer += Time.deltaTime;
                await UniTask.Yield(destroyCancellationToken);
            }

            idx = -1;
            foreach (var element in _accumulatedScoreList)
            {
                element.SetYPosition(idx * -AccumulatedScoreYOffset);
                idx++;
            }
        }
    }
}