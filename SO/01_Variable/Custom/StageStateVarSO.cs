using System;
using UnityEngine;

[CreateAssetMenu(menuName = "TaeBoMi/Custom Var/StageState", fileName = "StageStateVarSO", order = 1100)]
public class StageStateVarSO : ScriptableObject
{
    public Action<StageState> OnStateChanged;
    
    public StageState State { get; private set; }

    public void Init(StageState state)
    {
        State = state;
    }

    public void Set(StageState state)
    {
        State = state;
        OnStateChanged?.Invoke(state);
    }
}

public enum StageState
{
    None,
    Init,
    Start,
    Playing,
    GameOver,
}
