using System.Threading;
using Cysharp.Threading.Tasks;
using TaeBoMi.Pool;
using UnityEngine;

public class SummonEffect : MonoBehaviour
{
    [Header("Variable SO - Getter")]
    [SerializeField] private Vector3VarSO overlordMoveSpeedVarSO;
    
    private IObjectPool<SummonEffect> _pool;

    private CancellationTokenSource _disableCts;

    private void OnEnable()
    {
        _disableCts?.Dispose();
        _disableCts = new CancellationTokenSource();
    }

    private void OnDisable()
    {
        _disableCts.Cancel();
    }

    private void Update()
    {
        transform.Translate(overlordMoveSpeedVarSO.Value * Time.deltaTime);
    }

    public void Initialize(IObjectPool<SummonEffect> pool)
    {
        _pool = pool;
    }

    public void Set(Vector3 pos)
    {
        transform.position = pos;
        gameObject.SetActive(true);
    }

    public async UniTask WaitForSummonTiming()
    {
        await UniTask.Delay(300, cancellationToken: _disableCts.Token);
    } 


    public void AniEvent_AniFinished()
    {
        if (_pool is not null)
        {
            gameObject.SetActive(false);
            _pool.Release(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
