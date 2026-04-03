using System.Collections.Generic;
using SOSG.UI;
using UnityEngine;

public class GashaponQueueUI : MonoBehaviour
{
    [Header("Listener")][SerializeField] private IntEnumerableEventSO capsuleQueueChangedEventSO;
    
    [Header("Data")][SerializeField] private GashaponDataSO capsuleDataSO;
    
    [SerializeField] private List<MonsterIconUIElement> monsterIconList;
    // todo queueSizeSO의 개수에 맞춰서 증가시켜주기
    [SerializeField] private MonsterIconUIElement monsterIconOri;
    
    private void Awake()
    {
        capsuleQueueChangedEventSO.OnEventRaised += OnCapsuleLevelQueueChanged;
    }


    private void OnDestroy()
    {
        capsuleQueueChangedEventSO.OnEventRaised -= OnCapsuleLevelQueueChanged;
    }


    private void OnCapsuleLevelQueueChanged(IEnumerable<int> levelQueue)
    {
        var i = 0;
        foreach (var level in levelQueue)
        {
            monsterIconList[i].Set(capsuleDataSO.monsterCapsuleDataArr[level]);
            i++;
        }
    }
}