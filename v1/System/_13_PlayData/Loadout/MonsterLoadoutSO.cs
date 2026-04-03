using System;
using Cysharp.Threading.Tasks;
using SOSG.Monster;
using UnityEngine;

namespace SOSG.System.PlayData
{
    [CreateAssetMenu(menuName = "SOSG/Play Data/Monster Loadout Var")]
    public class MonsterLoadoutSO : ScriptableObject
    {
        [NonSerialized] public MonsterDataSO[] tierArr;

        public async UniTask SetUpAsync(string[] monsterIdArr)
        {
            tierArr = new MonsterDataSO[monsterIdArr.Length];
            for (var i = 0; i < tierArr.Length; i++)
            {
                tierArr[i] = await MonsterDataSOLoader.LoadMonsterDataSOAsync(monsterIdArr[i]);
            }
        }

        public void Release()
        {
            if (tierArr is not null)
            {
                foreach (var monsterDataSO in tierArr)
                {
                    
                }

                tierArr = null;
            }
        }
    }
}