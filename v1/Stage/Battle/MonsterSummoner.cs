using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using SOSG.Monster;
using SOSG.System.PlayData;
using TaeBoMi.Pool;
using UnityEngine;

public class MonsterSummoner : MonoBehaviour
{
    [Header("Event SO - Broadcaster")]
    [SerializeField] private MonsterRequestEventSO monsterRequestEventSO;

    [Header("Variable SO")]
    [SerializeField] private PlayerLoadoutVarSO playerLoadoutVarSO;
    
    
    [SerializeField] private SummonEffect summonEffectPrefab;

    private IObjectPool<SummonEffect> _summonEffectPool;

    private List<MonsterController> _monsters;

    private CancellationTokenSource _destroyCts;

    private int Row = 18;
    private int Col = 7;

    private void Awake()
    {
        _summonEffectPool = new ObjectPool<SummonEffect>(CreateEffect);
        _monsters = new List<MonsterController>(Row * Col);

        _destroyCts = new CancellationTokenSource();
        return;

        SummonEffect CreateEffect()
        {
            var effect = Instantiate(summonEffectPrefab, transform);
            effect.Initialize(_summonEffectPool);
            return effect;
        }
    }

    private void OnDestroy()
    {
        _destroyCts.CancelAndDispose();
    }


    public void SummonMonster(int level)
    {
        Summon(level).Forget();
    }

    private async UniTaskVoid Summon(int level)
    {
        if (_monsters.Count < Row * Col)
        {
            var idx = _monsters.Count;
            var row = idx / Col;
            var col = idx - row * Col;
            var summonPos = transform.position - new Vector3(row + 1.5f, col - 4f);
            var effect = _summonEffectPool.Get();
            effect.Set(summonPos);
            var monster = monsterRequestEventSO.RequestMonster(level);
            _monsters.Add(monster);
            await effect.WaitForSummonTiming();

            monster.SetPosition(summonPos);
        }
        else
        {
            foreach (var monster in _monsters)
            {
                if (monster.CurLevel < level)
                {
                    var monsterPos = monster.transform.position;
                    var effect = _summonEffectPool.Get();
                    effect.Set(monsterPos);
                    await effect.WaitForSummonTiming();
                    monster.Set(playerLoadoutVarSO.data.MonsterLoadout[level], level);
                    break;
                }
            }
        }
    }
}