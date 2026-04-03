using System;
using CodeStage.AntiCheat.ObscuredTypes;
using SOSG.Stage;
using UnityEngine;

namespace SOSG.Stage.Suika.Combo
{
    [CreateAssetMenu(menuName = "SOSG/Suika/Combo", fileName = "SuikaComboSO")]
    public class SuikaComboSO : ScriptableObject
    {
        private ObscuredInt _value;
        public int Value { get; private set; }
        public ComboGrade Grade { get; private set; }

        public event Action ComboChanged;

        public void SetUp()
        {
            _value = 0;
        }

        public void Increase()
        {
            _value++;
            UpdateValue();
        }

        public void ResetCombo()
        {
            _value = 0;
            UpdateValue();
        }

        private void UpdateValue()
        {
            Value = _value;
            Grade = ComboUtility.GetGrade(Value);
            ComboChanged?.Invoke();
        }
    }
}