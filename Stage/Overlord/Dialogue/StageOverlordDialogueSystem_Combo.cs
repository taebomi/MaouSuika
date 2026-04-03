using System;
using SOSG.Stage;
using SOSG.Stage.Suika.Combo;
using SOSG.System.Localization;
using UnityEngine;

namespace SOSG.System.Dialogue
{
    public partial class StageOverlordDialogueSystem
    {
        [Header("Combo")]
        [SerializeField] private GashaponComboSO curComboSO;
        [SerializeField] private GashaponComboSO highComboSO;
        [SerializeField] private GashaponComboSO lastComboSO;
        [SerializeField] private VoidEventSO comboFailedEventSO;

        private ComboGrade _lastComboGrade;

        private void AwakeCombo()
        {
            curComboSO.OnValueChanged += OnCurComboChanged;
            comboFailedEventSO.OnEventRaised += OnComboFailed;
        }

        private void OnDestroyCombo()
        {
            curComboSO.OnValueChanged -= OnCurComboChanged;
            comboFailedEventSO.OnEventRaised -= OnComboFailed;
        }

        // 대사가 출력중이지 않을 경우에만 콤보 대사 출력
        private void OnCurComboChanged()
        {
            var grade = curComboSO.Grade;
            if (grade is ComboGrade.None)
            {
                _lastComboGrade = ComboGrade.None;
                return;
            }

            // todo 수정
            // if (_lastComboGrade == grade && dialogueHelper.IsLineDataListEmpty is false)
            // {
            //     return;
            // }

            switch (grade)
            {
                case ComboGrade.Low:
                    const string lowComboLineKey = "low-combo";
                    const int lowComboLineCount = 7;
                    RequestRandomLine(LocalizationTableName.Stage_System, lowComboLineKey, lowComboLineCount, StageOverlordLineType.Combo);
                    break;
                case ComboGrade.Mid:
                    const string midComboLineKey = "mid-combo";
                    const int midComboLineCount = 5;
                    RequestRandomLine(LocalizationTableName.Stage_System, midComboLineKey, midComboLineCount, StageOverlordLineType.Combo);
                    break;
                case ComboGrade.High:
                case ComboGrade.Extreme: // todo extreme 등급 추가해주기
                    const string highComboLineKey = "high-combo";
                    const int highComboLineCount = 3;
                    RequestRandomLine(LocalizationTableName.Stage_System, highComboLineKey, highComboLineCount, StageOverlordLineType.Combo);
                    break;
                case ComboGrade.None:
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _lastComboGrade = grade;
        }

        private void OnComboFailed()
        {
            if (curComboSO.IsCombo is false)
            {
                return;
            }

            switch (curComboSO.Grade)
            {
                case ComboGrade.Low:
                    const string lowComboLineKey = "low-combo-failed";
                    const int lowComboLineCount = 7;
                    RequestRandomLine(LocalizationTableName.Stage_System, lowComboLineKey, lowComboLineCount, StageOverlordLineType.Combo);
                    break;
                case ComboGrade.Mid:
                    const string midComboLineKey = "mid-combo-failed";
                    const int midComboLineCount = 5;
                    RequestRandomLine(LocalizationTableName.Stage_System, midComboLineKey, midComboLineCount, StageOverlordLineType.Combo);
                    break;
                case ComboGrade.High:
                case ComboGrade.Extreme: // todo extreme 등급 추가해주기
                    const string highComboLineKey = "high-combo-failed";
                    const int highComboLineCount = 3;
                    RequestRandomLine(LocalizationTableName.Stage_System, highComboLineKey, highComboLineCount, StageOverlordLineType.Combo);
                    break;
                case ComboGrade.None:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}