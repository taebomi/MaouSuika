using System;
using UnityEngine;

[CreateAssetMenu(menuName = "TaeBoMi/Custom Event/Request Gashapon Explode Effect", fileName = "RequestGashaponExplodeEffectEventSO", order = 3500)]
public class GashaponExplodeEffectRequestEventSO : ScriptableObject
{
    public Func<int, GashaponExplodeEffectBase> OnRequestGashaponExplodeEffect;
    
    public GashaponExplodeEffectBase Request(int level)
    {
        return OnRequestGashaponExplodeEffect?.Invoke(level);
    }
}
