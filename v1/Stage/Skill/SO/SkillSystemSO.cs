using System;
using UnityEngine;

namespace SOSG.Skill
{
    [CreateAssetMenu(menuName = "TaeBoMi/Skill/Skill System SO", fileName = "SkillSystemSO")]
    public class SkillSystemSO : ScriptableObject
    {
        public Action OnSkillUIClicked;
        public Action<float> OnSkillGaugeChanged;

        public Action OnSkillActivated;
        public Action OnSkillDeactivated;

        public Action OnSkillGaugeChargeRequested;

        public Action<bool> OnAdIconSetEnableRequested;
        

        public void NotifySkillGaugeChanged(float ratio)
        {
            OnSkillGaugeChanged?.Invoke(ratio);
        }

        public void OnSkillUiClicked()
        {
            OnSkillUIClicked?.Invoke();
        }
        
        public void NotifySkillActivated()
        {
            OnSkillActivated?.Invoke();
        }
        
        public void NotifySkillDeactivated()
        {
            OnSkillDeactivated?.Invoke();
        }

        public void RequestChargeSkillGauge()
        {
            OnSkillGaugeChargeRequested?.Invoke();
        }

        public void NotifyRewardedAdReady(bool value)
        {
            OnAdIconSetEnableRequested?.Invoke(value);
        }

    }
}