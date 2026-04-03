using System.Collections.Generic;
using SOSG.System.Localization;
using SOSG.System.MonsterUnlock;
using SOSG.System.PlayData;
using SOSG.UI;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace SOSG.Customization.Monster
{
    public class MonsterUnlock : MonoBehaviour
    {
        [SerializeField] private MonsterUnlockProgressVarSO unlockProgressVarSO;

        [SerializeField] private MonsterCustomizationManagerSO managerSO;
        [SerializeField] private MagiSO magiSO;

        [SerializeField] private MonsterCustomizationSceneManager sceneManager;
        [SerializeField] private MonsterSummoner summoner;
        [SerializeField] private MonsterSummon summon;
        [SerializeField] private MonsterSummonScreen summonScreen;

        // [SerializeField] public LocalizationHelper LocalizationHelper { get; private set; }

        private List<MonsterInfo> _unlockableMonsterInfoList;
        private bool HasUnlockedAll => _unlockableMonsterInfoList.Count is 0;

        private MonsterInfo _summonedMonsterInfo;

        private const int MagiCost = 300;

        private void Awake()
        {
            SetUpCallbacks();
        }

        private void OnDestroy()
        {
            TearDownCallbacks();
        }

        private void SetUpCallbacks()
        {
            managerSO.OnUnlockBtnClicked += OnUnlockBtnClicked;
        }

        private void TearDownCallbacks()
        {
            managerSO.OnUnlockBtnClicked -= OnUnlockBtnClicked;
        }

        public void Initialize(List<MonsterInfo> unlockableMonsterInfoList)
        {
            _unlockableMonsterInfoList = unlockableMonsterInfoList;

            summon.Initialize(OnSummonUIClosed);
            summonScreen.Initialize();
        }

        private void OnUnlockBtnClicked()
        {
            if (HasUnlockedAll)
            {
                managerSO.NotifyUnlockUIOpenFailedAllUnlocked();
                return;
            }

            Activate();
        }

        public void Activate()
        {
            summoner.Activate(HasUnlockedAll);
            summon.Open().Forget();
            managerSO.NotifyUnlockUIOpenSucceed();
        }

        private void OnSummonUIClosed()
        {
            summoner.Deactivate();
        }

        public void OnResultClosing()
        {
            summon.OnSummonFinished();
        }

        public void OnSummonBtnClicked()
        {
            if (HasUnlockedAll)
            {
                managerSO.NotifySummonStartFailedAllUnlocked();
                return;
            }

            // 마기 체크
            if (magiSO.Magi < MagiCost)
            {
                FailSummon();
            }
            else
            {
                SuccessSummon();
            }
        }

        private void SuccessSummon()
        {
            var randomIdx = Random.Range(0, _unlockableMonsterInfoList.Count);
            _summonedMonsterInfo = _unlockableMonsterInfoList[randomIdx];
            _summonedMonsterInfo.SetUnlocked();
            _unlockableMonsterInfoList.RemoveAt(randomIdx);

            var summonedMonsterData = _summonedMonsterInfo.MonsterData;

            magiSO.Subtract(MagiCost);
            unlockProgressVarSO.Unlock(summonedMonsterData.id);
            unlockProgressVarSO.RequestSave();

            summoner.SetSummonState(summonedMonsterData);
            summon.OnSummonStarted();
            summonScreen.OnSummonStarted();
            managerSO.NotifySummonStarted();
        }

        private void FailSummon()
        {
            managerSO.NotifySummonStartFailedNoMagi();
        }

        #region Monster Summon

        public void Signal_Summon_StartEffect() => summoner.ActivateAllSummonEffects();

        public void Signal_Summon_StopEffect() => summoner.StopSummonEffect();

        public void Signal_Summon_Summoning1() => managerSO.NotifySummoning1();

        public void Signal_Summon_Summoning2() => managerSO.NotifySummoning2();

        public void Signal_Summon_SummonMonster()
        {
            // var monsterData = _summonedMonsterInfo.MonsterData;
            // var monsterName =
            //     LocalizationHelper.GetLocalizedValue(LocalizationTableName.MonsterName,
            //         monsterData.id);
            // var description =
            //     LocalizationHelper.GetLocalizedValue("summon-result-description", ("monster-name", monsterName));
            // summonScreen.ShowResultScreen(monsterData, description);
            //
            // summoner.SetSummonSucceedState(monsterData);
            // managerSO.NotifySummonSucceed(_summonedMonsterInfo);
        }

        public void Signal_Summon_Finished() => summoner.DeactivateFadingCanvas();

        #endregion
    }
}