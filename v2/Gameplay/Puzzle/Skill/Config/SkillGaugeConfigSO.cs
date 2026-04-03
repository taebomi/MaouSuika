using System;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    [CreateAssetMenu(fileName = "SkillGaugeConfig", menuName = "Maou Suika/Puzzle/Skill/Config")]
    public class SkillGaugeConfigSO : ScriptableObject
    {
        [Serializable]
        public struct ComboGaugeEntry
        {
            [Tooltip("이 콤보 수 이하일 때 적용")]
            public int maxCombo;
            public float gaugeAmount;
        }

        [Tooltip("콤보 수 오름차순으로 정렬. 첫 번째로 충족하는 항목이 적용됨.")]
        [SerializeField] private ComboGaugeEntry[] comboGaugeTable;

        public float GetGaugeAmount(int combo)
        {
            foreach (var entry in comboGaugeTable)
            {
                if (combo <= entry.maxCombo)
                    return entry.gaugeAmount;
            }

            return comboGaugeTable.Length > 0
                ? comboGaugeTable[comboGaugeTable.Length - 1].gaugeAmount
                : 0f;
        }
    }
}
