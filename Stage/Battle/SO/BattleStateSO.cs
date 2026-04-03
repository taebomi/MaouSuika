using System;
using UnityEngine;



[CreateAssetMenu(menuName = "TaeBoMi/Battle/State", fileName = "BattleStateSO", order = 1100)]
public class BattleStateSO : ScriptableObject
{
    public enum State
    {
        Stop,
        Move,
        Battle,
    }

    public State Value { get; private set; }
    public Action<State> OnStateChanged;
    
    public void ChangeState(State state)
    {
        Value = state;
        OnStateChanged?.Invoke(state);
    }
}
