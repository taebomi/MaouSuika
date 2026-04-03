using SOSG.Monster;
using SOSG.System.PlayData;
using TaeBoMi.Pool;
using UnityEngine;
using UnityEngine.Serialization;

public class MonsterPool : MonoBehaviour
{
    [Header("Event SO - Listener")]
    [SerializeField] private MonsterRequestEventSO monsterRequestEventSO;

    [Header("Variable SO")]
    [SerializeField] private PlayerLoadoutVarSO playerLoadoutVarSO;
    
    
    [FormerlySerializedAs("monsterPrefab")] [SerializeField] private MonsterController monsterControllerPrefab;

    private IObjectPool<MonsterController> _monsterPool;

    private void Awake()
    {
        _monsterPool = new ObjectPool<MonsterController>(CreateMonster);
        return;

        MonsterController CreateMonster()
        {
            var monster = Instantiate(monsterControllerPrefab, transform);
            monster.Initialize(_monsterPool);
            return monster;
        }
    }

    private void OnEnable()
    {
        monsterRequestEventSO.FuncOnRequestMonster += GetMonster;
    }

    private void OnDisable()
    {
        monsterRequestEventSO.FuncOnRequestMonster -= GetMonster;
    }

    private MonsterController GetMonster(int level)
    {
        var monster = _monsterPool.Get();
        monster.Set(playerLoadoutVarSO.data.MonsterLoadout[level], level);
        return monster;
    }
}