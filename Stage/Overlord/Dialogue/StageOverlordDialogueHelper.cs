using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using SOSG.System.Dialogue;
using SOSG.System.Localization;
using TaeBoMi;
using UnityEngine;
using Random = UnityEngine.Random;

public class StageOverlordDialogueHelper : MonoBehaviour
{
    // private LinkedList<StageOverlordLineData> _lineDataList;
    // private StageOverlordLineData CurLineData => _lineDataList.First?.Value;
    // public bool IsLineDataListEmpty => _lineDataList.Count == 0;
    //
    // public override UniTask SetUp(LocalizationTableName tableName)
    // {
    //     _lineDataList = new LinkedList<StageOverlordLineData>();
    //     OnLineFinished = CheckLineDataList;
    //     return base.SetUp(tableName);
    // }
    //
    // public void RequestLine(LocalizationTableName tableName, string key, StageOverlordLineType lineType)
    // {
    //     TBMUtility.Log($"# Key - {key}");
    //     var line = LocalizationHelper.GetLocalizedValue(tableName, key);
    //     var lineData = new StageOverlordLineData(line, lineType);
    //
    //     switch (lineType)
    //     {
    //         case StageOverlordLineType.Normal:
    //             if (IsLineDataListEmpty)
    //             {
    //                 dialogueSystem.RequestLine(lineData);
    //             }
    //
    //             _lineDataList.AddLast(lineData);
    //
    //             break;
    //         case StageOverlordLineType.Combo:
    //             if (IsLineDataListEmpty)
    //             {
    //                 dialogueSystem.RequestLine(lineData);
    //             }
    //
    //             break;
    //         case StageOverlordLineType.GameOver:
    //             if (CurLineData is { IsConjunctionAdded: false })
    //             {
    //                 AddConjunction(CurLineData);
    //             }
    //
    //             dialogueSystem.RequestLine(lineData);
    //
    //             break;
    //         default:
    //             throw new ArgumentOutOfRangeException(nameof(lineType), lineType, null);
    //     }
    // }
    //
    // public void OnStageStarted()
    // {
    //     SetMainDialogueSystem(true);
    // }
    //
    //
    // public void OnStageEnded()
    // {
    //     SetMainDialogueSystem(false);
    //     _lineDataList.Clear();
    // }
    //
    // private void CheckLineDataList(LineData lineData)
    // {
    //     if (CurLineData == lineData)
    //     {
    //         _lineDataList.RemoveFirst();
    //     }
    //
    //     if (IsLineDataListEmpty is false)
    //     {
    //         var nextLineData = _lineDataList.First.Value;
    //         dialogueSystem.RequestLine(nextLineData);
    //     }
    // }
    //
    // private void AddConjunction(StageOverlordLineData lineData)
    // {
    //     const int conjunctionCount = 3;
    //     var randIdx = Random.Range(1, conjunctionCount + 1);
    //     var key = $"conjunction-{randIdx}";
    //     var conjunction = GetLocalizedValue(key);
    //     lineData.Text = $"{conjunction} {lineData.Text}";
    //     lineData.IsConjunctionAdded = true;
    // }
}