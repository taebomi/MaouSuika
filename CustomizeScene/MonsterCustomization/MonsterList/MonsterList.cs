using System.Collections.Generic;
using SOSG.Monster;
using UnityEngine;
using UnityEngine.Serialization;

namespace SOSG.Customization.Monster
{
    public class MonsterList : MonoBehaviour
    {
        [SerializeField] private MonsterCustomizationManagerSO managerSO;
        [FormerlySerializedAs("manager")] [SerializeField] private MonsterCustomizationSceneManager sceneManager;

        [SerializeField] private MonsterListUI monsterListUI;

        private Dictionary<MonsterGrade, List<MonsterInfo>> _gradeToListDataDict;
        private MonsterInfo _selectedMonsterInfo;

        #region Init Task

        private void Awake()
        {
            monsterListUI.Initialize(OnListElementClicked);
            SetUpCallbacks();
        }

        private void OnDestroy()
        {
            TearDownCallbacks();
        }

        public void Initialize(List<MonsterInfo> monsterInfoList)
        {
            // 등급별 dict 생성
            _gradeToListDataDict = new Dictionary<MonsterGrade, List<MonsterInfo>>();
            var monsterGradeList = MonsterGradeCache.CreateMonsterGradeListInstance();
            foreach (var monsterGrade in monsterGradeList)
            {
                _gradeToListDataDict.Add(monsterGrade, new List<MonsterInfo>());
            }

            // dict에 데이터 추가
            foreach (var monsterInfo in monsterInfoList)
            {
                _gradeToListDataDict[monsterInfo.MonsterData.grade].Add(monsterInfo);
            }

            ShowList(MonsterGrade.Common);
        }

        private void SetUpCallbacks()
        {
            managerSO.OnGradeSelectionChanged += OnGradeSelectionChanged;
            managerSO.OnLoadoutIconClicked += OnLoadoutIconClicked;
            managerSO.OnGetSelectedMonsterInfo += SelectedMonsterInfo;
            managerSO.OnSummonSucceed += OnMonsterSummoned;
            managerSO.OnMonsterEquipSucceed += OnMonsterEquipSucceed;
        }

        private void TearDownCallbacks()
        {
            managerSO.OnGradeSelectionChanged -= OnGradeSelectionChanged;
            managerSO.OnLoadoutIconClicked -= OnLoadoutIconClicked;
            managerSO.OnGetSelectedMonsterInfo -= SelectedMonsterInfo;
            managerSO.OnSummonSucceed -= OnMonsterSummoned;
            managerSO.OnMonsterEquipSucceed -= OnMonsterEquipSucceed;
        }

        #endregion


        public void OnListElementClicked(MonsterInfo clickedMonsterInfo)
        {
            managerSO.NotifyMonsterListElementClicked(clickedMonsterInfo);
        }


        #region Event

        private void OnGradeSelectionChanged(MonsterGrade grade) => ShowList(grade);

        private void OnLoadoutIconClicked(MonsterInfo monsterInfo)
        {
            var monsterData = monsterInfo.MonsterData;
            ShowList(monsterData.grade);
            SelectMonster(monsterInfo);
        }

        private void OnMonsterSummoned(MonsterInfo summonedMonsterInfo)
        {
            var monsterData = summonedMonsterInfo.MonsterData;
            ShowList(monsterData.grade);
            SelectMonster(summonedMonsterInfo);
        }


        private void OnMonsterEquipSucceed() => RefreshUI();

        #endregion


        private void RefreshUI() => monsterListUI.RefreshUI();
        private void ShowList(MonsterGrade grade) => monsterListUI.ShowList(_gradeToListDataDict[grade]);
        private void SelectMonster(MonsterInfo monsterInfo) => monsterListUI.Select(monsterInfo);
        private MonsterInfo SelectedMonsterInfo() => monsterListUI.SelectedMonsterInfo;
    }
}