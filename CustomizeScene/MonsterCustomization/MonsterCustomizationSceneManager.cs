using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using SOSG.System.Dialogue;
using SOSG.Monster;
using SOSG.System;
using SOSG.System.Localization;
using SOSG.System.MonsterUnlock;
using SOSG.System.PlayData;
using SOSG.System.Scene;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Serialization;

namespace SOSG.Customization.Monster
{
    public class MonsterCustomizationSceneManager : MonoBehaviour
    {
        [SerializeField] private MonsterCustomizationManagerSO managerSO;

        [SerializeField] private MonsterUnlockProgressVarSO unlockProgressVarSO;
        [SerializeField] private PlayerLoadoutVarSO playerLoadoutVarSO;

        [SerializeField] private MonsterList monsterList;
        [SerializeField] private MonsterUnlock monsterUnlock;
        [SerializeField] private MonsterLoadout monsterLoadout;
        [SerializeField] private MonsterDetail monsterDetail;


        private List<MonsterInfo> _monsterInfoList;
        private Dictionary<MonsterDataSO, MonsterInfo> _monsterInfoDict;
        private List<MonsterInfo> _equippedMonsterInfoList;
        private List<MonsterInfo> _unlockableMonsterInfoList;

        private AsyncOperationHandle<IList<MonsterDataSO>> _allMonsterDataHandle;

        // private LocalizationHelper _localizationHelper;
        private TempDialogueHelper _tempDialogueHelper;
        
        private void Awake()
        {
            // _localizationHelper = GetComponent<LocalizationHelper>();
            _tempDialogueHelper = GetComponent<TempDialogueHelper>();
             SceneSetUpHelper.AddTask(Initialize);
        }

        private void OnDestroy()
        {
            MonsterDataSOLoader.ReleaseMonsterDataSO(_allMonsterDataHandle);
        }


        private async UniTask Initialize()
        {
            _allMonsterDataHandle = MonsterDataSOLoader.LoadAllMonsterDataSO();
            var monsterDataList = await _allMonsterDataHandle;
            InitializeMonsterInfo(monsterDataList);

            monsterList.Initialize(_monsterInfoList);
            monsterUnlock.Initialize(_unlockableMonsterInfoList);
            monsterLoadout.Initialize(_equippedMonsterInfoList);
            // await monsterDetail.Initialize();
            // await _localizationHelper.SetUpAsync(LocalizationTableName.MonsterCustomization);


            monsterDetail.ShowDetail(_monsterInfoDict[playerLoadoutVarSO.monsterLoadout[0]]);
        }

        private void InitializeMonsterInfo(IList<MonsterDataSO> monsterDataList)
        {
            _monsterInfoList = new List<MonsterInfo>();
            _monsterInfoDict = new Dictionary<MonsterDataSO, MonsterInfo>();
            _unlockableMonsterInfoList = new List<MonsterInfo>();
            foreach (var monsterData in monsterDataList)
            {
                var isUnlocked = unlockProgressVarSO.IsUnlocked(monsterData.id);
                var monsterInfo = new MonsterInfo(monsterData, isUnlocked);
                _monsterInfoList.Add(monsterInfo);
                _monsterInfoDict.Add(monsterData, monsterInfo);
                if (isUnlocked is false)
                {
                    _unlockableMonsterInfoList.Add(monsterInfo);
                }
            }

            _equippedMonsterInfoList = new List<MonsterInfo>();
            var equippedMonsterArr = playerLoadoutVarSO.monsterLoadout;
            for (var tier = 0; tier < System.PlayData.MonsterLoadout.Size; tier++)
            {
                var monsterData = equippedMonsterArr[tier];
                var monsterInfo = _monsterInfoDict[monsterData];
                monsterInfo.SetEquipped(tier);
                _equippedMonsterInfoList.Add(monsterInfo);
            }
        }
    }
}