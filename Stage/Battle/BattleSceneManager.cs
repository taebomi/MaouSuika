using UnityEngine;

public class BattleSceneManager : MonoBehaviour
{
    [SerializeField] private VoidEventSO stageStartEventSO;
    [SerializeField] private VoidEventSO stageEndEventSO;

    [Header("Variable SO")]
    [SerializeField] private BattleStateSO battleStateSO;

    private void Awake()
    {
        stageStartEventSO.OnEventRaised += OnStageStarted;
        stageEndEventSO.OnEventRaised += OnStageEnded;
    }

    private void OnDestroy()
    {
        stageStartEventSO.OnEventRaised -= OnStageStarted;
        stageEndEventSO.OnEventRaised -= OnStageEnded;
    }

    private void OnStageStarted()
    {
        battleStateSO.ChangeState(BattleStateSO.State.Move);
    }

    private void OnStageEnded()
    {
        battleStateSO.ChangeState(BattleStateSO.State.Stop);
    }

    private void CreateNextBiomeChunk()
    {
        // 현재 점수가 바이옴 전환 점수보다 높을 경우 바이옴 데이터 전환
    }

    private void ChangeBiome()
    {
        // curBiomeDataVarSO 전환 및 이벤트 날림
    }
}