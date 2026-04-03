using System.Collections.Generic;
using System.Linq;
using SOSG.Monster;
using SOSG.System.PlayData;
using UnityEngine;
using UnityEngine.Serialization;

namespace SOSG.Customization.Monster
{
    public class MonsterLoadout : MonoBehaviour
    {
        [SerializeField] private MonsterCustomizationManagerSO managerSO;

        [SerializeField] private PlayerLoadoutVarSO loadoutVarSO;

        [SerializeField] private MonsterLoadoutMainUI mainUI;
        [FormerlySerializedAs("iconUI")] [SerializeField] private MonsterLoadoutIcon icon;

        private bool IsEquipping { get; set; }

        private List<MonsterInfo> _equippedMonsterInfoList;
        private MonsterInfo _selectedMonsterInfo;

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
            managerSO.OnGradeSelectionChanged += OnGradeSelectionChanged;
            managerSO.OnEquipBtnClicked += OnEquipBtnClicked;
            managerSO.OnSummonSucceed += OnMonsterSummoned;
        }

        private void TearDownCallbacks()
        {
            managerSO.OnGradeSelectionChanged -= OnGradeSelectionChanged;
            managerSO.OnEquipBtnClicked -= OnEquipBtnClicked;
            managerSO.OnSummonSucceed -= OnMonsterSummoned;
        }

        public void Initialize(List<MonsterInfo> equippedMonsterInfoList)
        {
            _equippedMonsterInfoList = equippedMonsterInfoList;
            icon.UpdateIcons(equippedMonsterInfoList);
            mainUI.SetGrade(MonsterGrade.Common);

            IsEquipping = false;
        }

        private void ApplyToLoadout()
        {
        }

        public void SetGrade(MonsterGrade grade)
        {
            mainUI.SetGrade(grade);
        }

        public void OnEquipBtnClicked()
        {
            _selectedMonsterInfo = managerSO.GetSelectedMonsterInfo();
            IsEquipping = true;

            mainUI.StartBlink();
            icon.ActivateBlackBackground();
        }

        public void OnIconClicked(MonsterInfo clickedMonsterInfo)
        {
            if (IsEquipping is false)
            {
                SetGrade(clickedMonsterInfo.MonsterData.grade);
                managerSO.NotifyLoadoutIconClicked(clickedMonsterInfo);
            }
            else
            {
                EquipMonster(clickedMonsterInfo);
            }
        }

        public void OnEquipCanceled()
        {
            IsEquipping = false;
            mainUI.StopBlink();
            icon.DeactivateBlackBackground();
            managerSO.NotifyMonsterEquipCanceled();
        }

        private void EquipMonster(MonsterInfo clickedMonsterInfo)
        {
            var clickedMonsterData = clickedMonsterInfo.MonsterData;
            var selectedMonsterData = _selectedMonsterInfo.MonsterData;
            // 등급이 다를 경우 
            if (clickedMonsterData.grade != selectedMonsterData.grade)
            {
                managerSO.NotifyMonsterEquipFailed_NotSameGrade();
                return;
            }

            // 동일 몬스터 클릭 시
            if (clickedMonsterData == selectedMonsterData)
            {
                managerSO.NotifyMonsterEquipFailed_SameClicked();
                return;
            }
            
            IsEquipping = false;

            // 선택 몬스터가 이미 장착중이었으면 클릭된 몬스터의 위치 교체
            var clickedTier = clickedMonsterInfo.EquippedTier;
            if (_selectedMonsterInfo.IsEquipped)
            {
                var selectedTier = _selectedMonsterInfo.EquippedTier;
                EquipMonster(selectedTier, clickedMonsterInfo);
            }
            else
            {
                clickedMonsterInfo.SetUnequipped();
            }
            EquipMonster(clickedTier, _selectedMonsterInfo);
            
            mainUI.StopBlink();
            icon.DeactivateBlackBackground();

            managerSO.NotifyMonsterEquipSucceed();
        }

        private void OnGradeSelectionChanged(MonsterGrade grade) => SetGrade(grade);
        private void OnMonsterSummoned(MonsterInfo monsterInfo) => SetGrade(monsterInfo.MonsterData.grade);

        private void EquipMonster(int tier, MonsterInfo monsterInfo)
        {
            _equippedMonsterInfoList[tier] = monsterInfo;
            monsterInfo.SetEquipped(tier);
            loadoutVarSO.ChangeMonsterLoadout(tier, monsterInfo.MonsterData);
            icon.UpdateIcon(tier, monsterInfo);
        }
    }
}