using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using SOSG.Stage;
using SOSG.Stage.Area;
using SOSG.System;
using SOSG.System.Scene;
using UnityEngine;
using StageAreaDataSO = SOSG.Stage.Area.StageAreaDataSO;

namespace SOSG.Area
{
    public class StageAreaManager : MonoBehaviour
    {
        [SerializeField] private StageAreaManagerSO stageAreaManagerSO;
        [SerializeField] private StageAreaDataSO stageAreaDataSO;

        [SerializeField] private ObscuredIntVarSO curScoreVarSO;

        [SerializeField] private StageAreaDataApplier areaDataApplier;
        [SerializeField] private BattleAreaManager battleAreaManager;
        [SerializeField] private GashaponAreaManager gashaponAreaManager;

        private Queue<Func<UniTask>> _areaChangeTaskQueue;
        private bool _isChangingArea;

        public const float AreaChangeDuration = 1f;

        private void Awake()
        {
            _areaChangeTaskQueue = new Queue<Func<UniTask>>();

            curScoreVarSO.OnValueChanged += OnScoreChanged;
            stageAreaManagerSO.ActionOnAreaChanged += OnAreaChanged;

            SceneSetUpHelper.AddTask(SetUpAsync);
        }

        private async UniTask SetUpAsync()
        {
            var initAreaData = stageAreaDataSO.data.areaData[0];
            stageAreaManagerSO.Initialize(initAreaData);
            areaDataApplier.Initialize(initAreaData);
            await UniTask.WhenAll(battleAreaManager.Initialize(initAreaData),
                gashaponAreaManager.Initialize(initAreaData));
        }

        private void OnDestroy()
        {
            curScoreVarSO.OnValueChanged -= OnScoreChanged;
            stageAreaManagerSO.ActionOnAreaChanged -= OnAreaChanged;
        }

        private void OnScoreChanged(int curScore)
        {
            var curAreaData = stageAreaDataSO.GetAreaData(curScore);
            if (curAreaData == stageAreaManagerSO.CurAreaData)
            {
                return;
            }

            stageAreaManagerSO.SetCurAreaData(curAreaData);
        }

        private void OnAreaChanged(AreaData newAreaData)
        {
            _areaChangeTaskQueue.Enqueue(() => ChangeArea(newAreaData));
            if (_isChangingArea is false)
            {
                ChangeArea().Forget();
            }
        }

        private async UniTask ChangeArea()
        {
            _isChangingArea = true;
            while (_areaChangeTaskQueue.TryDequeue(out var taskFunc))
            {
                await taskFunc();
            }

            _isChangingArea = false;
        }

        private async UniTask ChangeArea(AreaData newAreaData)
        {
            await UniTask.WhenAll(battleAreaManager.CreateNextAreaAsync(newAreaData),
                gashaponAreaManager.CreateNextAreaAsync(newAreaData));

            await UniTask.WhenAll(battleAreaManager.ChangeAreaAsync(),
                gashaponAreaManager.ChangeAreaAsync());
        }
    }
}