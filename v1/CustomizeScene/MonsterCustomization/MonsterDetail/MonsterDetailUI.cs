using SOSG.Monster;
using TaeBoMi;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SOSG.Customization.Monster
{
    public class MonsterDetailUI : MonoBehaviour
    {
        [SerializeField] private Image monsterNameContainerImage;
        [SerializeField] private TMP_Text monsterNameText;
        [SerializeField] private TMP_Text statsText;
        
        [SerializeField] private Sprite[] monsterNameContainerSpriteArr;

        [SerializeField] private MonsterDetailUIPreviewer previewer;
        [SerializeField] private MonsterDetailDescriptionUIElement descriptionUIElement;
        
        public void Initialize(string lockedDescription)
        {
            descriptionUIElement.Initialize(lockedDescription);
        }

        public void SetLocked(MonsterDataSO monsterData)
        {
            var gradeIdx = TBMUtility.GetEnumIndex(monsterData.grade);
            monsterNameContainerImage.sprite = monsterNameContainerSpriteArr[gradeIdx];
            previewer.Set(monsterData, false);
            monsterNameText.text = "???";
            descriptionUIElement.SetNotUnlocked();
            statsText.text = "HP : ??? | ATK : ??? | SPD : ???";
        }

        public void SetUnlocked(MonsterDataSO monsterData, string monsterName, string description)
        {
            var gradeIdx = TBMUtility.GetEnumIndex(monsterData.grade);
            monsterNameContainerImage.sprite = monsterNameContainerSpriteArr[gradeIdx];
            previewer.Set(monsterData, true);
            monsterNameText.text = monsterName;
            descriptionUIElement.SetDescription(description);
            statsText.text =
                $"HP : {monsterData.stats.hp} | ATK : {monsterData.stats.atk} | SPD : {monsterData.stats.spd}";
        }
    }
}