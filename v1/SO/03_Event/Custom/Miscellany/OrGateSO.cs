using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TaeBoMi/Gate/Or", fileName = "OrSO", order = 30000)]
public class OrGateSO : ScriptableObject
{
    public Action<bool> ActionOnToggled;
    private Dictionary<object, bool> _toggleDict;

    private bool _lastValue;

    public void Invoke()
    {
        var result = false;
        foreach (var (_, value) in _toggleDict)
        {
            if (value)
            {
                result = true;
                break;
            }
        }
        
        ActionOnToggled?.Invoke(result);
        _lastValue = result;
    }
    
    public void AddOnAwake(object toggle)
    {
        _toggleDict ??= new Dictionary<object, bool>();
        _toggleDict.Add(toggle, false);
    }

    public void AddAfterAwake(object toggle, bool value)
    {
        _toggleDict ??= new Dictionary<object, bool>();
        _toggleDict.Add(toggle, value);
        Check();
    }

    public void Remove(object toggle)
    {
        _toggleDict.Remove(toggle);
        Check();
    }

    public void Set(object toggle, bool value)
    {
        _toggleDict[toggle] = value;
        Check();
    }

    private void Check()
    {
        var result = false;
        foreach (var (_, value) in _toggleDict)
        {
            if (value)
            {
                result = true;
                break;
            }
        }

        if (result == _lastValue)
        {
            return;
        }

        ActionOnToggled?.Invoke(result);
        _lastValue = result;
    }
}