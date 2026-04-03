using System;
using UnityEngine;

namespace SOSG.Stage.Suika.Combo
{
    [CreateAssetMenu(fileName = "ComboEffectPoolSO", menuName = "SOSG/Suika/Combo/Effect Pool")]
    public class ComboEffectPoolSO : ScriptableObject
    {
        public event Func<Vector3, ComboEffect> GetRequested;
        public event Action<ComboEffect> ReturnRequested;

        public ComboEffect Get(Vector3 pos)
        {
            return GetRequested?.Invoke(pos);
        }

        public void Return(ComboEffect comboEffect)
        {
            if (ReturnRequested == null)
            {
                Destroy(comboEffect.gameObject);
            }
            else
            {
                ReturnRequested.Invoke(comboEffect);
            }
        }
    }
}