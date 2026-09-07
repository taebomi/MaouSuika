using System;
using MaouSuika.Core;
using UnityEngine;

namespace MaouSuika.Gameplay
{
    [Serializable]
    public class ComboGradeSettings
    {
        public Color color;

        public ComboIndicatorSettings indicatorSettings;
    }

    [Serializable]
    public class ComboPopupSettings
    {
        public AnimationCurve scaleByCombo;
        public AnimationCurve holdTimeByCombo;
    }

    [Serializable]
    public class ComboIndicatorSettings
    {
        public ParticleSystem.MinMaxCurve startSize;
        public ParticleSystem.MinMaxCurve emissionRate;
        public ParticleSystem.MinMaxGradient colorOverLifetime;
    }

    public readonly struct ComboIndicatorStyle
    {
        public readonly Color Color;
        public readonly ParticleSystem.MinMaxCurve StartSize;
        public readonly ParticleSystem.MinMaxCurve EmissionRate;
        public readonly ParticleSystem.MinMaxGradient ColorOverLifetime;

        public ComboIndicatorStyle(Color color, ComboIndicatorSettings settings)
        {
            Color = color;
            StartSize = settings.startSize;
            EmissionRate = settings.emissionRate;
            ColorOverLifetime = settings.colorOverLifetime;
        }
    }

    [CreateAssetMenu(fileName = "Combo_StyleConfigSO", menuName = MenuPath.Gameplay.COMBO + "Style Config")]
    public class ComboStyleConfigSO : ScriptableObject
    {
        [SerializeField] private ComboPopupSettings popupSettings;

        [SerializeField] private ComboGradeSettings normalSettings;

        [SerializeField] private ComboGradeSettings highSettings;

        [SerializeField] private ComboGradeSettings maxSettings;

        public ComboPopupStyle PopupStyleFor(int combo, ComboGrade grade)
        {
            combo = Mathf.Clamp(
                combo,
                ComboGrades.NORMAL_MIN_COMBO,
                ComboGrades.MAX_MIN_COMBO);
            var gradeSettings = GradeSettingsFor(grade);

            return new ComboPopupStyle(
                popupSettings.scaleByCombo.Evaluate(combo),
                gradeSettings.color,
                popupSettings.holdTimeByCombo.Evaluate(combo));
        }

        public ComboIndicatorStyle IndicatorStyleFor(ComboGrade grade)
        {
            var settings = GradeSettingsFor(grade);
            return new ComboIndicatorStyle(settings.color, settings.indicatorSettings);
        }

        private ComboGradeSettings GradeSettingsFor(ComboGrade grade)
        {
            return grade switch
            {
                ComboGrade.Normal => normalSettings,
                ComboGrade.High => highSettings,
                ComboGrade.Max => maxSettings,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
