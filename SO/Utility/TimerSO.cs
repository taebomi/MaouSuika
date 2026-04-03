using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TimerSO : MonoBehaviour
{
    public float RemainedTime { get; private set; }
    public float SetTime { get; private set; }
    
    public State CurState { get; private set; }

    
    public Action ActionOnTimerCanceled;
    public Action ActionOnTimerFinished;
    
#if UNITY_EDITOR
    [TextArea, SerializeField] private string memo;
    [SerializeField] private bool debugMode;
#endif



    public void Set(float time)
    {
        SetTime = time;
    }

    public async UniTask StartTimer(CancellationToken ct)
    {
        while (RemainedTime > 0f && ct.IsCancellationRequested is false)
        {
            if (CurState is not State.Running)
            {
                return;
            }
            
            RemainedTime -= Time.deltaTime;
            await UniTask.Yield(ct);
        }
    }

    public void Restart()
    {
        RemainedTime = SetTime;
    }

    public void Pause()
    {
        ChangeState(State.Idle);
    }
    
    private void ChangeState(State state)
    {
        if (state == CurState)
        {
            return;
        }
    }

    public enum State
    {
        Idle,
        Running,
        Finished,
    }
}
