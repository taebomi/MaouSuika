using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using SOSG.Stage.Suika.Score;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace SOSG.Stage.UI
{
    public class ScoreUI : MonoBehaviour
    {
        [Header("Event SO")]
        [SerializeField] private IntEventSO scoreGetEventSO;
        [SerializeField] private VoidEventSO comboFailedEventSO;

        [Header("Variable SO")]
        [SerializeField] private IntVariableSO bestScoreVarSO;
        
        [SerializeField] private StageStateVarSO stageStateVarSO;

    
        [FormerlySerializedAs("accumulatedScorePrefab")]
        [Header("Prefab")]
        [SerializeField] private AccumulatedScoreUIElement accumulatedScoreUIElementPrefab;
        
        [Header("Components")]
        [SerializeField] private TextMeshProUGUI bestScoreText;
        [SerializeField] private TextMeshProUGUI curScoreText;
        [SerializeField] private Transform accumulatedScoreContainerTr;

        private AccumulatedScoreUIElement _curAccumulatedScoreUIElement;
        private LinkedList<AccumulatedScoreUIElement> _needToAddAccumulatedScoreList;

        private int _curScore;

        private bool _isAddingAccumulatedScore;

        private CancellationTokenSource _destroyCts;

        private const float AccumulatedScoreYOffset = 40f;


        private void Awake()
        {
            _destroyCts = new CancellationTokenSource();

            _needToAddAccumulatedScoreList = new LinkedList<AccumulatedScoreUIElement>();
            _curScore = 0;
            _curAccumulatedScoreUIElement = null;

        }

        private void Start()
        {
            bestScoreText.text = $"BEST {bestScoreVarSO.Value:00000}";
        }

        private void OnEnable()
        {
            scoreGetEventSO.OnEventRaised += OnScoreGet;
            comboFailedEventSO.OnEventRaised += OnComboFailed;
            stageStateVarSO.OnStateChanged += OnStageStateChanged;
        }

        private void OnDisable()
        {
            scoreGetEventSO.OnEventRaised -= OnScoreGet;
            comboFailedEventSO.OnEventRaised -= OnComboFailed;
            stageStateVarSO.OnStateChanged -= OnStageStateChanged;
        }

        private void OnDestroy()
        {
            _destroyCts.CancelAndDispose();
        }
    
        private void OnStageStateChanged(StageState state)
        {
            if (state is StageState.GameOver)
            {
                GameOver();
            }
        }

        private void GameOver()
        {
            OnComboFailed();
        }

        private void OnScoreGet(int getScore)
        {
            if (_curAccumulatedScoreUIElement is null)
            {
                _curAccumulatedScoreUIElement =
                    Instantiate(accumulatedScoreUIElementPrefab, accumulatedScoreContainerTr, false);
                if (_needToAddAccumulatedScoreList.Count > 0)
                {
                    var yPos = (_needToAddAccumulatedScoreList.Last.Value.CurYPos - AccumulatedScoreYOffset);
                    _curAccumulatedScoreUIElement.SetYPosition(yPos);
                }
                else
                {
                    _curAccumulatedScoreUIElement.SetYPosition(0f);
                }

                _needToAddAccumulatedScoreList.AddLast(_curAccumulatedScoreUIElement);
            }

            _curAccumulatedScoreUIElement.AddScore(getScore);
        }

        private void OnComboFailed()
        {
            if (_curAccumulatedScoreUIElement)
            {
                _curAccumulatedScoreUIElement.Emphasize();
                _curAccumulatedScoreUIElement = null;
            }

            if (_isAddingAccumulatedScore || _needToAddAccumulatedScoreList.Count == 0)
            {
                return;
            }

            AddAccumulatedScore().Forget();
        }

        private async UniTaskVoid AddAccumulatedScore()
        {
            _isAddingAccumulatedScore = true;
            while (_needToAddAccumulatedScoreList.Count > 0)
            {
                var accumulatedScore = _needToAddAccumulatedScoreList.First.Value;
                if (accumulatedScore.IsAccumulating)
                {
                    break;
                }

                await AddScoreFromAccumulatedScore(accumulatedScore);
                accumulatedScore.MakeDisappear();
                _needToAddAccumulatedScoreList.RemoveFirst();
                if (_needToAddAccumulatedScoreList.Count > 0)
                {
                    await ReplaceAccumulatedScores();
                }

            }

            _isAddingAccumulatedScore = false;
        }

        private async UniTask AddScoreFromAccumulatedScore(AccumulatedScoreUIElement accumulatedScoreUIElement)
        {
            var remainedScore = accumulatedScoreUIElement.Score;
            var timer = 0f;
            var addScore = 0f;
            while (remainedScore > 0 && _destroyCts.IsCancellationRequested is false)
            {
                var second = (int)timer;
                const int defaultIncreaseSpeed = 125;
                var speed = defaultIncreaseSpeed << second;
                addScore += speed * Time.deltaTime;

                var intAddScore = (int)addScore;
                addScore -= intAddScore;
                if (remainedScore < intAddScore)
                {
                    break;
                }

                remainedScore -= intAddScore;
                _curScore += intAddScore;
                curScoreText.text = $"{_curScore:00000}";
                accumulatedScoreUIElement.UpdateText(remainedScore);

                timer += Time.deltaTime;
                await UniTask.Yield(_destroyCts.Token);
            }

            _curScore += remainedScore;
            curScoreText.text = $"{_curScore:00000}";
            accumulatedScoreUIElement.UpdateText(0);
        }

        private async UniTask ReplaceAccumulatedScores()
        {
            int idx;
            var timer = 0f;
            const float duration = 0.5f;
            while (timer < duration && _destroyCts.IsCancellationRequested is false)
            {
                var yPos = AccumulatedScoreYOffset * Easing.InCubic(timer, duration);

                idx = 1;
                foreach (var accumulatedScore in _needToAddAccumulatedScoreList)
                {
                    accumulatedScore.SetYPosition(-AccumulatedScoreYOffset * idx + yPos);
                    idx++;
                }

                timer += Time.deltaTime;
                await UniTask.Yield(_destroyCts.Token);
            }

            idx = 0;
            foreach (var accumulatedScore in _needToAddAccumulatedScoreList)
            {
                accumulatedScore.SetYPosition(-AccumulatedScoreYOffset * idx);
                idx++;
            }
        }
    }
}