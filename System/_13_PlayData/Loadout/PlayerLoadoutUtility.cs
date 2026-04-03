using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SOSG.Monster;
using SOSG.System.PlayData;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace SOSG.System.Loadout
{
    public static class PlayerLoadoutUtility
    {
        public static AsyncOperationHandle<IList<MonsterDataSO>> GetMonsterLoadoutHandle(PlayerLoadoutString loadoutString)
        {
            var monsterString = loadoutString.monsterLoadoutString;
            var idArr = monsterString.monsterIdArr;
            var keyArr = idArr.Select(id => $"MonsterData/{id}").ToArray();
            var handle = Addressables.LoadAssetsAsync<MonsterDataSO>(keyArr, null, Addressables.MergeMode.Union);
            return handle;
        }
    }
}