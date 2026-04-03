using Cysharp.Threading.Tasks;
using SOSG.Stage.GameOver;
using SOSG.System;
using SOSG.System.Input;
using SOSG.System.Scene;
using UnityEngine;

namespace SOSG.Stage
{
    public class EndlessModeManager : MonoBehaviour
    {
        [SerializeField] private StageManagerSO stageManagerSO;
        [SerializeField] private GameInputSO gameInputSO;

        [SerializeField] private GameInputSO inputSO;

        [Header("Event SO - BroadCaster")]
        [SerializeField] private VoidEventSO stageStartEvent;
        [SerializeField] private VoidEventSO stageEndEvent;

        [Header("Event SO - Listener")]
        [SerializeField] private BoolEventSO shootInputEnableEventSO;
        [SerializeField] private GameOverSystemStateSO gameOverSystemStateSO;


        [Header("Variable SO - Setter")]
        [SerializeField] private StageStateVarSO stageStateVarSO;

        [Header("Components")]
        [SerializeField] private StagePlayStatisticsManager stagePlayStatisticsManager;

        private void OnEnable()
        {
            gameOverSystemStateSO.ActionOnGameOver += OnGameOver;
        }

        private void OnDisable()
        {
            gameOverSystemStateSO.ActionOnGameOver -= OnGameOver;
        }


        private void Start()
        {

            gameInputSO.InitializeStageControl();

            InitializeStatistics();

            shootInputEnableEventSO.RaiseEvent(true);
            stageManagerSO.StartStage();
            stageStateVarSO.Set(StageState.Start);
            stageStartEvent.RaiseEvent();
        }

        private void InitializeStatistics()
        {
            stagePlayStatisticsManager.Initialize();
        }

        private void OnGameOver()
        {
            stageEndEvent.RaiseEvent();
            stageStateVarSO.Set(StageState.GameOver);
            stageManagerSO.EndStage();

            stagePlayStatisticsManager.Save();
        }
    }
}