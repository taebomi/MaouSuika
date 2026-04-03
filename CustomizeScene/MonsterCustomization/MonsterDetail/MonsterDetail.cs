using Cysharp.Threading.Tasks;
using SOSG.System.Localization;
using UnityEngine;
using UnityEngine.Serialization;

namespace SOSG.Customization.Monster
{
    public class MonsterDetail : MonoBehaviour
    {
        [SerializeField] private MonsterCustomizationManagerSO managerSO;
        
        // [FormerlySerializedAs("localizationController")] [SerializeField] private LocalizationHelper localizationHelper;

        [SerializeField] private MonsterDetailUI ui;

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
            managerSO.OnMonsterListElementClicked += OnMonsterListElementClicked;
            managerSO.OnSummonSucceed += OnMonsterSummoned;
            managerSO.OnLoadoutIconClicked += OnLoadoutIconClicked;
        }
        
        private void TearDownCallbacks()
        {
            managerSO.OnMonsterListElementClicked -= OnMonsterListElementClicked;
            managerSO.OnSummonSucceed -= OnMonsterSummoned;
            managerSO.OnLoadoutIconClicked -= OnLoadoutIconClicked;
        }

        // public async UniTask Initialize()
        // {
        //     // await localizationHelper.SetUpAsync(LocalizationTableName.MonsterName,
        //     //     LocalizationTableName.MonsterCustomization);
        //     // localizationHelper.SetMainTable(LocalizationTableName.MonsterCustomization);
        //
        //     var lockedDescription = localizationHelper.GetLocalizedValue("locked-monster-description");
        //     ui.Initialize(lockedDescription);
        // }

        public void ShowDetail(MonsterInfo monsterInfo)
        {
            SetUI(monsterInfo);
        }
        
        private void OnMonsterListElementClicked(MonsterInfo monsterInfo) => ShowDetail(monsterInfo);
        private void OnMonsterSummoned(MonsterInfo summonedMonsterInfo) => ShowDetail(summonedMonsterInfo);
        
        private void OnLoadoutIconClicked(MonsterInfo monsterInfo) => ShowDetail(monsterInfo);

        private void SetUI(MonsterInfo elementInfo)
        {
            if (elementInfo.IsUnlocked)
            {
                // var monsterName =
                //     localizationHelper.GetLocalizedValue(LocalizationTableName.MonsterName,
                //         elementInfo.MonsterData.id);
                // var description =
                //     localizationHelper.GetLocalizedValue(LocalizationTableName.MonsterCustomization,
                //         $"{elementInfo.MonsterData.id}-description");
                // ui.SetUnlocked(elementInfo.MonsterData, monsterName, description);
            }
            else
            {
                ui.SetLocked(elementInfo.MonsterData);
            }
        }
    }
}