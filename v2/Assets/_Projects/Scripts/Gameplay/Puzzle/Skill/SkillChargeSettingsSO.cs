using MaouSuika.Core;
using UnityEngine;

namespace MaouSuika.Gameplay
{
    [CreateAssetMenu(fileName = "SkillChargeSettingsSO", menuName = MenuPath.Gameplay.SKILL + "Charge Settings")]
    public class SkillChargeSettingsSO : ScriptableObject
    {
        [Tooltip("인덱스 0: 최소 유효 콤보\n" +
                 "배열 크기 초과: 마지막 값 사용")]
        [SerializeField] private int[] gainByCombo;

        public int GainFor(int combo)
        {
            if (combo < ComboGrades.NORMAL_MIN_COMBO) return 0;

            var index = Mathf.Min(combo - ComboGrades.NORMAL_MIN_COMBO, gainByCombo.Length - 1);
            return gainByCombo[index];
        }
    }
}