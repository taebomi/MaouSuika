using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using SOSG.System.Dialogue;
using SOSG.Scene;
using SOSG.Stage;
using SOSG.System;
using SOSG.System.Localization;
using SOSG.System.PlayData;
using SOSG.System.Scene;
using SOSG.System.UI;
using SOSG.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class GameOverUI : MonoBehaviour
{
    [Header("Variable SO")] [SerializeField]
    private StagePlayStatisticsSO stageStatistics;

    [SerializeField] private ObscuredIntVarSO curScoreVarSO;
    [SerializeField] private IntVariableSO bestScoreVarSO;

    [SerializeField] private GashaponDataSO gashaponDataSO;

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private MonsterIconUIElement[] monsterIconArr;
    [SerializeField] private TMP_Text[] gashaponCountTextArr;
    [SerializeField] private GameObject newRecordGameObject;
    [SerializeField] private TMP_Text scoreText;

    private TempDialogueHelper _tempDialogueHelper;

    // todo 아이콘 받아와서 동기화 해주기
    // todo 세부 플레이 데이터도 표시해주기

    private void Awake()
    {
        gameObject.SetActive(false);
        canvasGroup.alpha = 1f;
        _tempDialogueHelper = GetComponent<TempDialogueHelper>();
    }

    public void OnRestartBtnClicked()
    {
        _tempDialogueHelper.RequestLine("restart");
        SceneLoadHelper.LoadScene(SceneName.EndlessMode);
    }

    public void OnHomeBtnClicked()
    {
        _tempDialogueHelper.RequestLine("home");
        SceneLoadHelper.LoadScene(SceneName.Main);
    }

    public async UniTaskVoid Activate()
    {
        SetData();
        gameObject.SetActive(true);
        canvasGroup.blocksRaycasts = false;
        await ShowUI();
        canvasGroup.blocksRaycasts = true;
    }

    private async UniTask ShowUI()
    {
        // modalUI.Show();
        await transform.DOScaleX(1f, 0.25f).From(0f).SetUpdate(true).SetEase(Ease.OutBack).Play()
            .AwaitForComplete(TweenCancelBehaviour.KillAndCancelAwait, destroyCancellationToken);
    }

    private void SetData()
    {
        var monsterDataArr = gashaponDataSO.monsterCapsuleDataArr;
        for (var i = 0; i < monsterDataArr.Length; i++)
        {
            var monsterData = monsterDataArr[i];
            monsterIconArr[i].Set(monsterData);
            gashaponCountTextArr[i].text = $"{stageStatistics.Data.createdMonsterCount[i]}";
        }

        scoreText.text = $"{curScoreVarSO.Value:00000}";
        newRecordGameObject.SetActive(bestScoreVarSO.Value <= curScoreVarSO.Value);
    }


    //
    // private async UniTask SetScore()
    // {
    //     var timer = 0f;
    //     var remainedScore = curScoreVarSO.Value;
    //     var addScore = 0f;
    //     var curScore = 0;
    //     while (remainedScore > 0 && _destroyCts.IsCancellationRequested is false)
    //     {
    //         var second = (int)timer;
    //         const int defaultIncreaseSpeed = 500;
    //         var speed = defaultIncreaseSpeed << second;
    //         addScore += speed * Time.deltaTime;
    //         
    //         var intAddScore = (int)addScore;
    //         if (remainedScore < intAddScore)
    //         {
    //             break;
    //         }
    //
    //         addScore -= intAddScore;
    //         remainedScore -= intAddScore;
    //         curScore += intAddScore;
    //         if (bestScoreVarSO.Value < curScore && !newRecordGameObject.activeSelf)
    //         {
    //             newRecordGameObject.SetActive(true);
    //         }
    //
    //         scoreText.text = $"{curScore:00000}";
    //         timer += Time.deltaTime;
    //         await UniTask.Yield(_destroyCts.Token);
    //     }
    //
    //
    //     curScore = curScoreVarSO.Value;
    //     scoreText.text = $"{curScore:00000}";
    //     if (bestScoreVarSO.Value < curScore && !newRecordGameObject.activeSelf)
    //     {
    //         newRecordGameObject.SetActive(true);
    //     }
    // }
    //
    // private async UniTask IncreaseGashaponCount(int level, int count)
    // {
    //     var timer = 0f;
    //     const float duration = 0.25f;
    //     while (timer < duration)
    //     {
    //         var curCount = (int)Mathf.Lerp(0, count, timer / duration);
    //         gashaponCountTextArr[level].text = $"{curCount}";
    //         timer += Time.deltaTime;
    //         await UniTask.Yield(_destroyCts.Token);
    //     }
    //
    //     gashaponCountTextArr[level].text = $"{count}";
    // }
}