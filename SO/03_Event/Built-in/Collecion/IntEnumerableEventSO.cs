using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TaeBoMi/Built-in Event/Int Enumerable", fileName = "IntEnumerableEventSO",order = 3100)]
public class IntEnumerableEventSO : ScriptableObject
{
    public Action<IEnumerable<int>> OnEventRaised;

    public void RaiseEvent(IEnumerable<int> intEnumerable)
    {
        OnEventRaised?.Invoke(intEnumerable);
    }
}