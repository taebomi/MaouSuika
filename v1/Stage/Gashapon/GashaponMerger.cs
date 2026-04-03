using System.Threading;
using Cysharp.Threading.Tasks;
using FMODUnity;
using SOSG.Stage;
using SOSG.System.Audio;
using UnityEngine;

public class GashaponMerger : MonoBehaviour
{
    [Header("Event SO - Sender")]
    [SerializeField] private GashaponRequestEventSO gashaponRequestEventSO;
    [SerializeField] private GashaponExplodeEffectRequestEventSO explodeEffectRequestEventSO;

    [Header("Event SO - Listener")]
    [SerializeField] private GashaponMergeEventSO gashaponMergeEventSO;
    [SerializeField] private IntEventSO gashaponMergedEventSO;
    
    [Header("Data SO")]
    [SerializeField] private GashaponDataSO gashaponDataSO;
    [SerializeField] private FloatArrDataSO gashaponSizeDataSO;

    [Header("Variable SO - Getter")]
    [SerializeField] private GashaponComboSO curCombo;

    [Header("SFX")]
    [SerializeField] private EventReference mergeSfx;


    private CancellationTokenSource _destroyCts;

    private void Awake()
    {
        _destroyCts = new CancellationTokenSource();

        gashaponMergeEventSO.OnEventRaised += OnMergeEventRaised;
    }

    private void OnDestroy()
    {
        _destroyCts.CancelAndDispose();

        gashaponMergeEventSO.OnEventRaised -= OnMergeEventRaised;
    }

    private void OnMergeEventRaised(Gashapon gashapon1, Gashapon gashapon2)
    {
        var refPoint = (Vector2)transform.position;
        var gashapon1Dist = Vector2.SqrMagnitude(gashapon1.Rb.position - refPoint);
        var gashapon2Dist = Vector2.SqrMagnitude(gashapon2.Rb.position - refPoint);

        if (gashapon1Dist <= gashapon2Dist)
        {
            Merge(gashapon2, gashapon1).Forget();
        }
        else
        {
            Merge(gashapon1, gashapon2).Forget();
        }
    }

    private async UniTaskVoid Merge(Gashapon from, Gashapon to)
    {
        from.SetMergeState();
        to.SetMergeState();

        var timer = 0f;
        const float duration = 0.2f;
        var firstPos = from.Rb.position;
        while (timer < duration && _destroyCts.IsCancellationRequested is false)
        {
            var easing = Easing.InSine(timer, duration);
            from.Rb.position = Vector2.Lerp(firstPos, to.Rb.position, easing);

            timer += Time.deltaTime;
            await UniTask.Yield(_destroyCts.Token);
        }

        from.Deactivate();
        to.Deactivate();

        var curLevel = from.CurLevel;
        gashaponMergedEventSO.RaiseEvent(curLevel);
        var nextLevel = Mathf.Clamp(curLevel + 1,
            0, gashaponDataSO.monsterCapsuleDataArr.Length - 1);
        var explodeEffect = explodeEffectRequestEventSO.Request(nextLevel);
        explodeEffect.SetSize(gashaponSizeDataSO.Data[nextLevel]);
        explodeEffect.SetColor(gashaponDataSO.monsterCapsuleDataArr[nextLevel].mergeEffectColor);
        explodeEffect.transform.position = to.transform.position;

        if (IsMaxLevel(curLevel))
        {
            return;
        }

        gashaponRequestEventSO.Request(curLevel + 1, to.transform.position);
        PlaySfx();
    }

    private void PlaySfx()
    {
        var pitch = 0.65f + curCombo.Value * 0.1f;
        AudioSystemHelper.PlaySfx(mergeSfx, pitch);
    }


    private bool IsMaxLevel(int level)
    {
        return level == gashaponDataSO.monsterCapsuleDataArr.Length - 1;
    }
}