using System;
using SOSG.Monster;
using SOSG.System.Localization;
using UnityEngine;
using UnityEngine.Serialization;

namespace SOSG.Customization.Monster
{
    public class MonsterCustomizationMainUIController : MonoBehaviour
    {
        [SerializeField] private MonsterCustomizationManagerSO managerSO;

        // [FormerlySerializedAs("localizationController")] [SerializeField] private LocalizationHelper localizationHelper;
        
        [SerializeField] private MainUI mainUI;
        [SerializeField] private MainBtn mainBtn;

        private MonsterGrade _curGrade;

        private void Awake()
        {
            _curGrade = MonsterGrade.Common;
            mainBtn.Initialize(OnMainBtnClicked);
            mainUI.Initialize(OnGradeBtnClicked);
            SetupCallbacks();
        }

        private void OnDestroy()
        {
            TearDownCallbacks();
        }

        private void SetupCallbacks()
        {
            managerSO.OnLoadoutIconClicked += OnLoadoutIconClicked;
            managerSO.OnMonsterListElementClicked += OnMonsterListElementClicked;
            managerSO.OnSummonSucceed += OnMonsterSummoned;
        }

        private void TearDownCallbacks()
        {
            managerSO.OnLoadoutIconClicked -= OnLoadoutIconClicked;
            managerSO.OnMonsterListElementClicked -= OnMonsterListElementClicked;
            managerSO.OnSummonSucceed -= OnMonsterSummoned;
        }
        
        private void OnGradeBtnClicked(MonsterGrade grade)
        {
            if (_curGrade == grade)
            {
                return;
            }

            ChangeGrade(grade);
            UpdateMainBtn(null);
            managerSO.NotifyGradeSelectionChanged(grade);
        }

        private void OnLoadoutIconClicked(MonsterInfo monsterInfo)
        {
            ChangeGrade(monsterInfo.MonsterData.grade);
            UpdateMainBtn(monsterInfo);
        }
        
        private void OnMonsterListElementClicked(MonsterInfo monsterInfo)
        {
            UpdateMainBtn(monsterInfo);
        }
        
        private void OnMonsterSummoned(MonsterInfo monsterInfo)
        {
            ChangeGrade(monsterInfo.MonsterData.grade);
            UpdateMainBtn(monsterInfo);
        }

        private void ChangeGrade(MonsterGrade grade)
        {
            _curGrade = grade;
            mainUI.SetGrade(grade);
        }

        private void UpdateMainBtn(MonsterInfo monsterInfo)
        {
            const string unlockBtnTextKey = "unlock-btn";
            const string equipBtnTextKey = "equip-btn";
        
            if (monsterInfo is null)
            {
                // var btnText = localizationHelper.GetLocalizedValue(unlockBtnTextKey);
                // mainBtn.SetText(btnText);
                mainBtn.Activate(MainBtn.Mode.Unlock).Forget();
            }
            else if (monsterInfo.IsEquipped || monsterInfo.IsUnlocked)
            {
                // var btnText = localizationHelper.GetLocalizedValue(equipBtnTextKey);
                // mainBtn.SetText(btnText);
                mainBtn.Activate(MainBtn.Mode.Equip).Forget();
            }
            else
            {
                // var btnText = localizationHelper.GetLocalizedValue(unlockBtnTextKey);
                // mainBtn.SetText(btnText);
                mainBtn.Activate(MainBtn.Mode.Unlock).Forget();
            }
        }

        private void OnMainBtnClicked(MainBtn.Mode mode)
        {
            switch (mode)
            {
                case MainBtn.Mode.Unlock:
                    managerSO.NotifyUnlockBtnClicked();
                    break;
                case MainBtn.Mode.Equip:
                    managerSO.NotifyEquipBtnClicked();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
        }
    }
}