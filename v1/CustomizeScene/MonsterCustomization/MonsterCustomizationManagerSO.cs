using System;
using SOSG.Monster;
using UnityEngine;

// ReSharper disable InconsistentNaming

namespace SOSG.Customization.Monster
{
    [CreateAssetMenu(fileName = "MonsterCustomizationManagerSO", menuName = "TaeBoMi/Customization/Monster")]
    public class MonsterCustomizationManagerSO : ScriptableObject
    {
        public Action<MonsterGrade> OnGradeSelectionChanged; // 사용자 클릭에 의해 선택된 등급이 변경될 때 호출
        public Action<MonsterInfo> OnLoadoutIconClicked; // 아이콘 클릭 시 호출 ( 장비 장착 중 x )
        public Action<MonsterInfo> OnMonsterListElementClicked;


        // EQUIP
        public Action OnEquipBtnClicked;
        public Action OnMonsterEquipFailed_SameClicked;
        public Action OnMonsterEquipFailed_NotSameGrade;
        public Action OnMonsterEquipCanceled;
        public Action OnMonsterEquipSucceed;

        // SUMMON
        public Action OnUnlockBtnClicked;
        public Action OnUnlockUIOpenSucceed;
        public Action OnUnlockUIOpenFailedAllUnlocked;
        public Action OnSummonStartSucceed;
        public Action OnSummonStartFailedAllUnlocked;
        public Action OnSummonStartFailedNoMagi;
        public Action OnSummoning1;
        public Action OnSummoning2;
        public Action<MonsterInfo> OnSummonSucceed; // 몬스터가 소환됨. ( 소환 연출 도중 )

        public Func<MonsterInfo> OnGetSelectedMonsterInfo;

        public void NotifyGradeSelectionChanged(MonsterGrade grade) => OnGradeSelectionChanged?.Invoke(grade);

        public void NotifyLoadoutIconClicked(MonsterInfo clickedMonsterInfo) =>
            OnLoadoutIconClicked?.Invoke(clickedMonsterInfo);

        public void NotifyMonsterListElementClicked(MonsterInfo monsterInfo) =>
            OnMonsterListElementClicked?.Invoke(monsterInfo);


        #region SUMMON

        public void NotifyUnlockBtnClicked() => OnUnlockBtnClicked?.Invoke();
        public void NotifyUnlockUIOpenSucceed() => OnUnlockUIOpenSucceed?.Invoke();
        public void NotifyUnlockUIOpenFailedAllUnlocked() => OnUnlockUIOpenFailedAllUnlocked?.Invoke();
        
        public void NotifySummonStartFailedAllUnlocked() => OnSummonStartFailedAllUnlocked?.Invoke();
        public void NotifySummonStartFailedNoMagi() => OnSummonStartFailedNoMagi?.Invoke();
        public void NotifySummonStarted() => OnSummonStartSucceed?.Invoke();
        
        public void NotifySummoning1() => OnSummoning1?.Invoke();
        public void NotifySummoning2() => OnSummoning2?.Invoke();
        public void NotifySummonSucceed(MonsterInfo monsterInfo) => OnSummonSucceed?.Invoke(monsterInfo);

        #endregion

        #region EQUIP

        public void NotifyEquipBtnClicked() => OnEquipBtnClicked?.Invoke();

        public void NotifyMonsterEquipFailed_SameClicked() => OnMonsterEquipFailed_SameClicked?.Invoke();

        public void NotifyMonsterEquipFailed_NotSameGrade() => OnMonsterEquipFailed_NotSameGrade?.Invoke();

        public void NotifyMonsterEquipCanceled() => OnMonsterEquipCanceled?.Invoke();

        public void NotifyMonsterEquipSucceed() => OnMonsterEquipSucceed?.Invoke();

        #endregion

        public MonsterInfo GetSelectedMonsterInfo() => OnGetSelectedMonsterInfo.Invoke();
    }
}