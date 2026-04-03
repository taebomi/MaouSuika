using UnityEngine;
using UnityEngine.UI;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    public class SkillGaugeView : MonoBehaviour
    {
        [SerializeField] private Slider gaugeSlider;

        public void Setup(float initialNormalized = 0f)
        {
            gaugeSlider.value = initialNormalized;
        }

        public void UpdateGauge(float normalizedValue)
        {
            gaugeSlider.value = normalizedValue;
        }
    }
}
