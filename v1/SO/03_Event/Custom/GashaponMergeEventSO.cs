using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "TaeBoMi/Custom Event/Gashapon Merge", fileName = "GashaponMergeEventSO", order = 3500)]
public class GashaponMergeEventSO : ScriptableObject
{
    public UnityAction<Gashapon, Gashapon> OnEventRaised;

    public void RaiseEvent(Gashapon capsule1, Gashapon capsule2)
    {
        OnEventRaised?.Invoke(capsule1, capsule2);
    }
}
