using Cysharp.Threading.Tasks;
using SOSG.Stage.Map;
using SOSG.System;
using SOSG.System.Scene;
using UnityEngine;
using UnityEngine.Serialization;

namespace SOSG.Stage.Area
{
    public class StageAreaManager : MonoBehaviour
    {
        [SerializeField] private StageAreaSO stageAreaSO;
        [SerializeField] private Map.StageAreaDataSO stageAreaDataSO;

        [SerializeField] private IntVariableSO curScoreVarSO;

        [Header("Components")] [SerializeField]
        private AreaDataApplier areaDataApplier;

        [SerializeField] private GashaponMapManager gashaponMapManager;
        [SerializeField] private BattleAreaManager battleAreaManager;

        private void Awake()
        {
            stageAreaSO.FuncOnGetNextStageAreaData += GetNextAreaData;

            SceneSetUpHelper.AddTask(SetUp);
        }

        private void OnDestroy()
        {
            stageAreaSO.FuncOnGetNextStageAreaData -= GetNextAreaData;
        }

        private void SetUp()
        {
            var firstAreaData = stageAreaDataSO.data.areaData[0];
            areaDataApplier.Initialize(firstAreaData);
            gashaponMapManager.Initialize(firstAreaData);
            battleAreaManager.Initialize(firstAreaData);
            stageAreaSO.InitializeCurAreaData(firstAreaData);
        }

        private AreaData GetNextAreaData() => stageAreaDataSO.GetAreaData(curScoreVarSO.Value);
    }
}