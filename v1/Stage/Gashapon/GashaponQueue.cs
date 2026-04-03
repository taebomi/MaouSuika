using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GashaponQueue : MonoBehaviour
{
    [Header("Event SO")]
    [SerializeField] private IntEnumerableEventSO queueChangedEventSO;

    [Header("Variable SO")]
    [SerializeField] private IntVariableSO queueSizeSO;

    private Queue<int> _levelQueue;

    private void Awake()
    {
        _levelQueue = new Queue<int>(queueSizeSO.Value);
        for (var i = 0; i < queueSizeSO.Value; i++)
        {
            Enqueue();
        }
    }

    private void Enqueue()
    {
        var level = GetRandomLevel();
        _levelQueue.Enqueue(level);
    }

    public int Dequeue()
    {
        if (_levelQueue.Count == 0)
        {
            return GetRandomLevel();
        }
        
        Enqueue();
        var level = _levelQueue.Dequeue();
        queueChangedEventSO.RaiseEvent(_levelQueue);
        return level;
    }

    private static int GetRandomLevel()
    {
        return Random.Range(0, 5);
    }
}