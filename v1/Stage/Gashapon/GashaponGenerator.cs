using QFSW.QC;
using TaeBoMi.Pool;
using UnityEngine;

public class GashaponGenerator : MonoBehaviour
{
    [Header("Event SO - Listener")]
    [SerializeField] private GashaponRequestEventSO gashaponRequestEventSO;

    [Header("Data SO")]
    [SerializeField] private GashaponDataSO gashaponDataSO;
    [SerializeField] private FloatArrDataSO gashaponSizeDataSO;

    [SerializeField] private Gashapon gashaponPrefab;

    private IObjectPool<Gashapon> _gashaponPool;

    private void Awake()
    {
        _gashaponPool = new ObjectPool<Gashapon>(() =>
        {
            var gashapon = Instantiate(gashaponPrefab, transform);
            gashapon.Initialize(_gashaponPool);
            return gashapon;
        });

        gashaponRequestEventSO.OnRequestGashapon += GetGashapon;
    }

    private void OnDestroy()
    {
        gashaponRequestEventSO.OnRequestGashapon -= GetGashapon;
    }


    private Gashapon GetGashapon(int level, Vector3 pos)
    {
        if (level > gashaponDataSO.monsterCapsuleDataArr.Length - 1)
        {
            return null;
        }

        var gashapon = _gashaponPool.Get();
        gashapon.SetPosition(pos);
        gashapon.Set(gashaponDataSO.monsterCapsuleDataArr[level], gashaponSizeDataSO.Data[level], level);
        gashapon.gameObject.SetActive(true);

        return gashapon;
    }


    [Command("create-gashapon")]
    public void CreateGashapon(int level, Vector3 pos)
    {
        var gashapon = GetGashapon(level, pos);
        gashapon.SetPosition(pos);
    }
}