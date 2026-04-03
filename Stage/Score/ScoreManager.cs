using SOSG.System.PlayData;
using UnityEngine;

namespace SOSG.Stage
{
    public class ScoreManager : MonoBehaviour
    {
        [Header("Event SO")]
        [SerializeField] private IntEventSO scoreGetEventSO;

        [SerializeField] private VoidEventSO stageEndEvent;
        [SerializeField] private IntEventSO capsuleMergedEventSO;

        [Header("Variable SO")]
        [SerializeField] private ObscuredIntVarSO curScore;
        [SerializeField] private IntVariableSO highScoreVarSO;

        [SerializeField] private GamePlayStatisticsSO gamePlayStatisticsSO;
        [SerializeField] private GashaponComboSO curComboVarSO;

        [Header("Data SO")]
        [SerializeField] private IntArrVariableSO gashaponScoreDataSO;
        [SerializeField] private FloatArrVariableSO scoreMultiplierDataSO;

        private float _scoreMultiplier;

        private void Awake()
        {
            curScore.Initialize(0);
            highScoreVarSO.Initialize(gamePlayStatisticsSO.data.highScore);

            capsuleMergedEventSO.OnEventRaised -= OnCapsuleMerged;
            capsuleMergedEventSO.OnEventRaised += OnCapsuleMerged;
            curComboVarSO.OnValueChanged += OnComboChanged;
            stageEndEvent.OnEventRaised += OnStageEnded;
        }

        private void OnDestroy()
        {
            capsuleMergedEventSO.OnEventRaised -= OnCapsuleMerged;
            curComboVarSO.OnValueChanged -= OnComboChanged;
            stageEndEvent.OnEventRaised -= OnStageEnded;
        }

        private void OnCapsuleMerged(int level)
        {
            var getScore = (int)(gashaponScoreDataSO.value[level] * _scoreMultiplier);
            curScore += getScore;
            scoreGetEventSO.RaiseEvent(getScore);
        }

        private void OnComboChanged()
        {
            var combo = Mathf.Clamp(curComboVarSO.Value, 0, scoreMultiplierDataSO.value.Length - 1);
            _scoreMultiplier = scoreMultiplierDataSO.value[combo];
        }

        private void OnStageEnded()
        {
            if (curScore.Value > highScoreVarSO.Value)
            {
                highScoreVarSO.Set(curScore.Value);
            }
        }
    }
}