using System;
using SOSG.Monster;
using TaeBoMi;
using UnityEngine;
using UnityEngine.UI;

namespace SOSG.Customization.Monster
{
    public class MainUI : MonoBehaviour
    {
        [SerializeField] private Image bgImage;
        [SerializeField] private Image outLineImage;
        [SerializeField] private Button[] gradeBtnArr;
        [SerializeField] private Sprite[] bgSpriteArr;
        [SerializeField] private Sprite[] outLineSpriteArr;
        [SerializeField] private Sprite[] gradeBtnSpriteArr;

        private event Action<MonsterGrade> OnGradeBtnClickedAction;

        public void Initialize(Action<MonsterGrade> onGradeBtnClicked)
        {
            OnGradeBtnClickedAction = onGradeBtnClicked;
        }

        private void OnDestroy()
        {
            OnGradeBtnClickedAction = null;
        }

        public void SetGrade(MonsterGrade grade)
        {
            var curIdx = TBMUtility.GetEnumIndex(grade);
            bgImage.sprite = bgSpriteArr[curIdx];
            outLineImage.sprite = outLineSpriteArr[curIdx];

            for (var i = 0; i < gradeBtnArr.Length; i++)
            {
                gradeBtnArr[i].image.sprite = i == curIdx
                    ? gradeBtnSpriteArr[i * 2 + 1]
                    : gradeBtnSpriteArr[i * 2];
            }
        }

        public void OnCommonBtnClicked() => OnGradeBtnClicked(MonsterGrade.Common);

        public void OnUncommonBtnClicked() => OnGradeBtnClicked(MonsterGrade.Uncommon);

        public void OnRareBtnClicked() => OnGradeBtnClicked(MonsterGrade.Rare);

        public void OnEpicBtnClicked() => OnGradeBtnClicked(MonsterGrade.Epic);

        private void OnGradeBtnClicked(MonsterGrade grade) => OnGradeBtnClickedAction?.Invoke(grade);
    }
}