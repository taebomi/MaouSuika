using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using SOSG.Stage;
using SOSG.System;
using SOSG.System.Localization;
using SOSG.System.Scene;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace SOSG.System.Dialogue
{
    public partial class StageOverlordDialogueSystem : MonoBehaviour
    {
        [SerializeField] private StageManagerSO stageManagerSO;


        private Dictionary<string, int> _lastLineIdxDict;

        // private LocalizationHelper _localizationHelper;
        private StageOverlordDialogueHelper _dialogueHelper;

        private void Awake()
        {
            AwakeCombo();
            AwakeGameOver();
            AwakeMonsterCount();
            stageManagerSO.ActionOnStageStarted += OnStageStarted;
            stageManagerSO.ActionOnStageEnded += OnStageEnded;

             // SceneSetUpHelper.AddTask(SetUpAsync);
        }

        private void OnDestroy()
        {
            OnDestroyCombo();
            OnDestroyGameOver();
            OnDestroyMonsterCount();
            stageManagerSO.ActionOnStageStarted -= OnStageStarted;
            stageManagerSO.ActionOnStageEnded -= OnStageEnded;
        }

        // private async UniTask SetUpAsync()
        // {
        //     // _lastLineIdxDict = new Dictionary<string, int>();
        //     // _localizationHelper = GetComponent<LocalizationHelper>();
        //     // _dialogueHelper = GetComponent<StageOverlordDialogueHelper>();
        //     // await _localizationHelper.SetUpAsync(LocalizationTableName.Stage_MonsterDescription,
        //     //     LocalizationTableName.Stage_System);
        // }

        /// <summary>
        /// 랜덤한 대사 출력.
        /// </summary>
        private void RequestRandomLine(LocalizationTableName tableName, string key, int randomLineCount,
            StageOverlordLineType lineType)
        {
            if (!_lastLineIdxDict.TryGetValue(key, out var lastLineIdx))
            {
                _lastLineIdxDict.Add(key, 0);
                lastLineIdx = 0;
            }

            int lineIdx;
            do
            {
                lineIdx = Random.Range(1, randomLineCount + 1);
            } while (lastLineIdx == lineIdx);

            _lastLineIdxDict[key] = lineIdx;
            RequestLine(tableName, $"{key}-{lineIdx}", lineType);
        }

        private void RequestLine(LocalizationTableName tableName, string key, StageOverlordLineType lineType)
        {
            // dialogueHelper.RequestLine(tableName, key, lineType);
        }
    }
}