using SOSG.Stage.Area;
using SOSG.System.PlayData;
using UnityEngine;
using AreaData = SOSG.Area.AreaData;

namespace SOSG.Stage
{
    public class StagePlayStatisticsManager : MonoBehaviour
    {
        [SerializeField] private VoidEventSO applyRequestEvent;
        [SerializeField] private VoidEventSO saveRequestEvent;

        [SerializeField] private StagePlayStatisticsSO playStatisticsSO;

        [SerializeField] private StageAreaManagerSO stageAreaManagerSO;
        [SerializeField] private VoidEventSO gashaponShotEvent;
        [SerializeField] private IntEventSO gashaponMergedEvent;
        [SerializeField] private ObscuredIntVarSO curScore;

        private void Awake()
        {
            gashaponShotEvent.OnEventRaised += OnGashaponShot;
            gashaponMergedEvent.OnEventRaised += OnGashaponMerged;
            curScore.OnValueChanged += OnScoreChanged;
            stageAreaManagerSO.ActionOnAreaChanged += OnAreaChanged;
        }

        private void OnDestroy()
        {
            gashaponShotEvent.OnEventRaised -= OnGashaponShot;
            gashaponMergedEvent.OnEventRaised -= OnGashaponMerged;
            curScore.OnValueChanged -= OnScoreChanged;
            stageAreaManagerSO.ActionOnAreaChanged -= OnAreaChanged;
        }

        public void Initialize()
        {
            playStatisticsSO.Initialize();
        }

        public void Save()
        {
            applyRequestEvent.RaiseEvent();
            saveRequestEvent.RaiseEvent();
        }

        private void OnAreaChanged(AreaData _)
        {
            playStatisticsSO.IncreaseStageIdx();
        }

        private void OnGashaponShot()
        {
            playStatisticsSO.IncreaseShotCount();
        }

        private void OnGashaponMerged(int level)
        {
            playStatisticsSO.IncreaseCreatedMonsterCount(level);
        }

        private void OnScoreChanged(int value)
        {
            playStatisticsSO.SetScore(value);
        }
    }
}