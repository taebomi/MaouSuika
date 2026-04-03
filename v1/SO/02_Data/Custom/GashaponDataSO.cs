using SOSG.Monster;
using UnityEngine;


// todo 플레이어 데이터로부터 읽어오도록
[CreateAssetMenu(menuName = "TaeBoMi/Custom Data/Gashapon", fileName = "GashaponDataSO", order = 9000)]
public class GashaponDataSO : ScriptableObject
{
    public MonsterDataSO[] monsterCapsuleDataArr;
}
