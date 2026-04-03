using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using SOSG.Monster;
using SOSG.System.Loadout;
using SOSG.System.PlayData;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace SOSG.Stage.Player
{
    public class PlayerLoadout : MonoBehaviour
    {
        [field: SerializeField] public MonsterDataSO[] MonsterDataArr { get; private set; }

        private AsyncOperationHandle<IList<MonsterDataSO>> _monsterDataHandle;

        private void OnDestroy()
        {
            if (_monsterDataHandle.IsValid())
            {
                _monsterDataHandle.Release();
            }
        }

        public async UniTask SetUpAsync(PlayerLoadoutString loadoutString)
        {
            _monsterDataHandle = PlayerLoadoutUtility.GetMonsterLoadoutHandle(loadoutString);
            await _monsterDataHandle;

            var result = _monsterDataHandle.Result;
            MonsterDataArr = new MonsterDataSO[result.Count];
            for (var i = 0; i < result.Count; i++)
            {
                MonsterDataArr[i] = result[i];
            }
        }
    }
}