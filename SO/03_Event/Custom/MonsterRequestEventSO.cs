using System;
using SOSG.Monster;
using UnityEngine;

[CreateAssetMenu(menuName = "TaeBoMi/Custom Event/Monster Request", fileName = "MonsterRequestEventSO", order = 3500)]
public class MonsterRequestEventSO : ScriptableObject
{
    public Func<int, MonsterController> FuncOnRequestMonster;

    public MonsterController RequestMonster(int level)
    {
        return FuncOnRequestMonster?.Invoke(level);
    }
}
