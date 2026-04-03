using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class SkillRewardedAdIconUI : MonoBehaviour
{
    [SerializeField] private Animator adIconAni;

    private CancellationTokenSource _enabledCts;
    private static readonly int ShineTriggerId = Animator.StringToHash("Shine");

    private void OnDestroy()
    {
        _enabledCts?.CancelAndDispose();
    }

    public void SetEnable(bool value)
    {
        if (value)
        {
            gameObject.SetActive(true);
            _enabledCts?.CancelAndDispose();
            _enabledCts = new CancellationTokenSource();
            ShineRandomly(_enabledCts.Token).Forget();
        }
        else
        {
            gameObject.SetActive(false);
            _enabledCts?.Cancel();
        }
    }

    private async UniTaskVoid ShineRandomly(CancellationToken ct)
    {
        const float minDelay = 1f;
        const float maxDelay = 5f;
        while (ct.IsCancellationRequested is false)
        {
            adIconAni.SetTrigger(ShineTriggerId);
            await UniTask.Delay(TimeSpan.FromSeconds(Random.Range(minDelay, maxDelay)), cancellationToken: ct);
        }
    }
}
