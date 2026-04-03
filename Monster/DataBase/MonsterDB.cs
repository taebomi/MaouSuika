using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using SOSG.Monster;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "TaeBoMi/DB/Monster", fileName = "Monster DB", order = 0)]
public class MonsterDB : ScriptableObject
{
    [field: SerializeField] public List<string> IdDB { get; private set; }
    [field: SerializeField] public SerializedDictionary<MonsterGrade, List<string>> IdDBByGrade { get; private set; }

    public string GetRandomMonsterId(MonsterGrade grade)
    {
        var list = IdDBByGrade[grade];
        var randomIndex = Random.Range(0, list.Count);
        return list[randomIndex];
    }
}