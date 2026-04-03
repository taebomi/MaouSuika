using System;
using UnityEngine;

namespace SOSG.Stage
{
    [CreateAssetMenu(menuName = "TaeBoMi/Custom Event/Gashapon/Shooter Control", fileName = "ShooterControlEventSO", order = 10000)]
    public class GashaponShooterControlEventSO : ScriptableObject
    {
        public Action<Vector2, float> ActionOnControl;

        public void RaiseEvent(Vector2 dir, float ratio)
        {
            ActionOnControl?.Invoke(dir, ratio);
        }
    }
}