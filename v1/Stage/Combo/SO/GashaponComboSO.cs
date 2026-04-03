using System;
using SOSG.Stage.Suika.Combo;
using UnityEngine;

namespace SOSG.Stage
{
    [CreateAssetMenu(fileName = "GashaponCombo", menuName = "TaeBoMi/Custom Var/Gashapon/Combo")]
    public class GashaponComboSO : ScriptableObject
    {
        public int Value { get; private set; }
        public ComboGrade Grade { get; private set; }
        
        public event Action OnValueChanged;

#if UNITY_EDITOR
        [SerializeField, TextArea] private string memo;
#endif

        public bool IsCombo => Grade is not ComboGrade.None;

        public void Initialize(int value)
        {
            Value = value;
            Grade = ComboGrade.None;
        }

        public void Increase() => Set(Value + 1);

        public void Refresh() => Set(0);

        public void Set(int value)
        {
            Value = value;
            CheckCombo();
            OnValueChanged?.Invoke();
        }

        private void CheckCombo()
        {
            Grade = Value switch
            {
                >= (int)ComboGrade.Extreme => ComboGrade.Extreme,
                >= (int)ComboGrade.High => ComboGrade.High,
                >= (int)ComboGrade.Mid => ComboGrade.Mid,
                >= (int)ComboGrade.Low => ComboGrade.Low,
                _ => ComboGrade.None
            };
        }
    }
}