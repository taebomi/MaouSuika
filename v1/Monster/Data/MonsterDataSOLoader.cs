using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using SOSG.Monster;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class MonsterDataSOLoader
{
    public static async UniTask<MonsterDataSO> LoadMonsterDataSOAsync(string id)
    {
        var dataSO = await Addressables.LoadAssetAsync<MonsterDataSO>($"MonsterData/{id}");
        return dataSO;
    }

    public static AsyncOperationHandle<IList<MonsterDataSO>> LoadAllMonsterDataSO()
    {
        var keys = new List<string> { "Monster" };
        var handle = Addressables.LoadAssetsAsync<MonsterDataSO>
            (keys, null, Addressables.MergeMode.Union, true);
        return handle;
    }
    public static void ReleaseMonsterDataSO(AsyncOperationHandle<IList<MonsterDataSO>> handle)
    {
        Addressables.Release(handle);
    }
}