using System;
using UnityEngine;
using UnityEngine.UI;

namespace MaouSuika.Gameplay
{
    public class SkillView : MonoBehaviour
    {
        [SerializeField] private Button skillButton;
        [SerializeField] private Slider chargeGauge;

        private Action _requestUse;

        public void Initialize(Action requestUse)
        {
            _requestUse = requestUse;
            skillButton.onClick.AddListener(OnSkillClicked);
        }

        private void OnDestroy()
        {
            skillButton.onClick.RemoveListener(OnSkillClicked);
        }

        public void SetCharge(float normalizedCharge)
        {
            chargeGauge.value = normalizedCharge;
        }

        public void SetInteractable(bool interactable)
        {
            skillButton.interactable = interactable;
        }

        private void OnSkillClicked()
        {
            _requestUse();
        }
    }
}