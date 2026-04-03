using System;
using UnityEngine;

[CreateAssetMenu(menuName = "TaeBoMi/Custom Event/Request Gashapon", fileName = "RequestGashaponEventSO", order = 3500)]
public class GashaponRequestEventSO : ScriptableObject
{
    public Func<int,Vector3, Gashapon> OnRequestGashapon;

    public Gashapon Request(int level, Vector3 pos)
    {
        return OnRequestGashapon?.Invoke(level, pos);
    }
}
