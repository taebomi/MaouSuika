using System;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    [CreateAssetMenu(fileName = "AutoFireConfigSO", menuName = "Maou Suika/Puzzle/Shooter/Auto Fire Config")]
    public class AutoFireConfigSO : ScriptableObject
    {
        [Header("Timer")]
        [field: SerializeField] public float TimeLimit { get; private set; } = 15f;
        [field: SerializeField] public float PreDelay { get; private set; } = 0.25f;

        [Header("Step")]
        [field: SerializeField] public float StepRadiusOffset { get; private set; } = 0.5f;
        [field: SerializeField] public float StepMinRadius { get; private set; } = 1f;
        [field: SerializeField] public float StepGap { get; private set; } = 0.25f;
        [Tooltip("남은 Countdown에 해당하는 색상." +
                 "0초거나 stepColors보다 클 경우 0번째 값(Default Color) 사용됨.")]
        [SerializeField] private Color[] stepColors;
        private Color stepDefaultColor => stepColors[0];
        [field: SerializeField] public float StepAppearDuration { get; private set; } = 0.25f;
        [field: SerializeField] public float StepDisappearDuration { get; private set; } = 0.25f;
        [field: SerializeField] public float StepPulseDuration { get; private set; } = 0.25f;
        [field: SerializeField] public float StepScaleOffset { get; private set; } = 1.25f;


        public Color GetColorByRemainingCount(int count)
        {
            if (stepColors.Length == 0)
            {
                Logger.Error($"Step Colors is Empty.");
                return Color.white;
            }

            if (count < 0 || count >= stepColors.Length) return stepDefaultColor;

            return stepColors[count];
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (stepColors.Length == 0)
            {
                stepColors = new[] { Color.white };
                Logger.Error($"Step Colors is Empty.");
            }
        }
#endif
    }
}