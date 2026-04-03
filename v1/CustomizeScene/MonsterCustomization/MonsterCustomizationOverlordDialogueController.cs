using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using SOSG.System.Dialogue;
using SOSG.Monster;
using SOSG.System;
using SOSG.System.Localization;
using SOSG.System.Scene;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace SOSG.Customization.Monster
{
    public class MonsterCustomizationOverlordDialogueController : MonoBehaviour
    {
        [SerializeField] private MonsterCustomizationManagerSO managerSO;

        // private LocalizationHelper _localizationHelper;
        private TempDialogueHelper _tempDialogueHelper;


        private void Awake()
        {
            SetUpCallbacks();
            SceneSetUpHelper.AddTask(SetUpAsync);
        }

        private void OnDestroy()
        {
            TearDownCallbacks();
        }

        private async UniTask SetUpAsync()
        {
            // _localizationHelper = GetComponent<LocalizationHelper>();
            _tempDialogueHelper = GetComponent<TempDialogueHelper>();
            // await _localizationHelper.SetUpAsync(LocalizationTableName.MonsterCustomization);
        }

        private void SetUpCallbacks()
        {
            managerSO.OnGradeSelectionChanged += OnGradeSelectionChanged;
            managerSO.OnLoadoutIconClicked += OnLoadoutIconClicked;
            managerSO.OnMonsterListElementClicked += OnMonsterListElementClicked;

            // EQUIP
            managerSO.OnEquipBtnClicked += OnEquipBtnClicked;
            managerSO.OnMonsterEquipFailed_SameClicked += OnMonsterEquipFailed_SameClicked;
            managerSO.OnMonsterEquipFailed_NotSameGrade += OnMonsterEquipFailed_NotSameGrade;
            managerSO.OnMonsterEquipCanceled += OnMonsterEquipCanceled;
            managerSO.OnMonsterEquipSucceed += OnMonsterEquipSucceed;

            // SUMMON
            managerSO.OnUnlockUIOpenSucceed += OnUnlockUIOpenSucceed;
            managerSO.OnUnlockUIOpenFailedAllUnlocked += OnUnlockUIOpenFailedAllUnlocked;
            managerSO.OnSummonStartSucceed += OnSummonStartSucceed;
            managerSO.OnSummonStartFailedAllUnlocked += OnSummonStartFailedAllUnlocked;
            managerSO.OnSummonStartFailedNoMagi += OnSummonStartFailedNoMagi;
            managerSO.OnSummoning1 += OnSummoning1;
            managerSO.OnSummoning2 += OnSummoning2;
            managerSO.OnSummonSucceed += OnSummonSucceed;
        }

        private void TearDownCallbacks()
        {
            managerSO.OnGradeSelectionChanged -= OnGradeSelectionChanged;
            managerSO.OnLoadoutIconClicked -= OnLoadoutIconClicked;
            managerSO.OnMonsterListElementClicked -= OnMonsterListElementClicked;

            // EQUIP
            managerSO.OnEquipBtnClicked -= OnEquipBtnClicked;
            managerSO.OnMonsterEquipFailed_SameClicked -= OnMonsterEquipFailed_SameClicked;
            managerSO.OnMonsterEquipFailed_NotSameGrade -= OnMonsterEquipFailed_NotSameGrade;
            managerSO.OnMonsterEquipCanceled -= OnMonsterEquipCanceled;
            managerSO.OnMonsterEquipSucceed -= OnMonsterEquipSucceed;

            // SUMMON
            managerSO.OnUnlockUIOpenSucceed -= OnUnlockUIOpenSucceed;
            managerSO.OnUnlockUIOpenFailedAllUnlocked -= OnUnlockUIOpenFailedAllUnlocked;
            managerSO.OnSummonStartSucceed -= OnSummonStartSucceed;
            managerSO.OnSummonStartFailedAllUnlocked -= OnSummonStartFailedAllUnlocked;
            managerSO.OnSummonStartFailedNoMagi -= OnSummonStartFailedNoMagi;
            managerSO.OnSummoning1 -= OnSummoning1;
            managerSO.OnSummoning2 -= OnSummoning2;
            managerSO.OnSummonSucceed -= OnSummonSucceed;
        }

        private void OnGradeSelectionChanged(MonsterGrade changedGrade)
        {
            const string commonKey = "common-grade-selected";
            const string uncommonKey = "uncommon-grade-selected";
            const string rareKey = "rare-grade-selected";
            const string epicKey = "epic-grade-selected";

            var key = changedGrade switch
            {
                MonsterGrade.Common => commonKey,
                MonsterGrade.Uncommon => uncommonKey,
                MonsterGrade.Rare => rareKey,
                MonsterGrade.Epic => epicKey,
                _ => string.Empty
            };

            _tempDialogueHelper.RequestLine(key);
        }

        private void OnLoadoutIconClicked(MonsterInfo clickedMonsterInfo) =>
            ShowSelectedMonsterLine(clickedMonsterInfo);

        private void OnMonsterListElementClicked(MonsterInfo clickedMonsterInfo) =>
            ShowSelectedMonsterLine(clickedMonsterInfo);

        private void ShowSelectedMonsterLine(MonsterInfo monsterInfo)
        {
            const string equippedKey = "equipped-monster-selected";
            const string unequippedKey = "unequipped-monster-selected";
            const string lockedKey = "locked-monster-selected";

            var key = monsterInfo.IsEquipped ? equippedKey : monsterInfo.IsUnlocked ? unequippedKey : lockedKey;
            key += $"-{Random.Range(1, 4)}";
            _tempDialogueHelper.RequestLine(key);
        }

        #region EQUIP

        private void OnEquipBtnClicked()
        {
            const string equipBtnClicked = "equip-btn-clicked";
            _tempDialogueHelper.RequestLine(equipBtnClicked);
        }

        private void OnMonsterEquipFailed_SameClicked()
        {
            const string key = "equip-failed-same-clicked";
            _tempDialogueHelper.RequestLine(key);
        }

        private void OnMonsterEquipFailed_NotSameGrade()
        {
            const string key = "equip-failed-not-same-grade";
            _tempDialogueHelper.RequestLine(key);
        }

        private void OnMonsterEquipCanceled()
        {
            const string key = "equip-canceled";
            _tempDialogueHelper.RequestLine(key);
        }

        private void OnMonsterEquipSucceed()
        {
            const string key = "equip-succeed";
            _tempDialogueHelper.RequestLine(key);
        }

        #endregion

        #region SUMMON

        private void OnUnlockUIOpenSucceed()
        {
            const string key = "unlock-ui-open-succeed";
            _tempDialogueHelper.RequestLine(key);
        }

        private void OnUnlockUIOpenFailedAllUnlocked()
        {
            const string key = "unlock-ui-open-failed-all-unlocked";
            _tempDialogueHelper.RequestLine(key);
        }

        private void OnSummonStartSucceed()
        {
            const string key = "summon-start-succeed";
            _tempDialogueHelper.RequestLine(key);
        }

        private void OnSummonStartFailedAllUnlocked()
        {
            const string key = "summon-start-failed-no-unlockable-monster";
            _tempDialogueHelper.RequestLine(key);
        }

        private void OnSummonStartFailedNoMagi()
        {
            const string key = "summon-start-failed-no-magi";
            _tempDialogueHelper.RequestLine(key);
        }

        private void OnSummoning1()
        {
            const string key = "summon-ing-1";
            _tempDialogueHelper.RequestLine(key);
        }

        private void OnSummoning2()
        {
            const string key = "summon-ing-2";
            _tempDialogueHelper.RequestLine(key);
        }

        private void OnSummonSucceed(MonsterInfo summonedMonsterInfo)
        {
            // const string key = "summon-succeed";
            // var monsterName =
            //     _localizationHelper.GetLocalizedValue(LocalizationTableName.MonsterName,
            //         summonedMonsterInfo.MonsterData.id);
            // var args = ("monster-name", monsterName);
            // _dialogueHelper.RequestLine(key, args);
        }

        #endregion
    }
}