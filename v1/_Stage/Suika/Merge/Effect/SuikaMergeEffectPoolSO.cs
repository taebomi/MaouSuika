using System;
using SOSG.Monster;
using UnityEngine;

namespace SOSG.Stage.Suika.Merge
{
    [CreateAssetMenu(fileName = "SuikaMergeEffectPoolSO", menuName = "SOSG/Suika/Merge/Merge Effect Pool")]
    public class SuikaMergeEffectPoolSO : ScriptableObject
    {
        public event Func<MonsterGrade, Vector3, SuikaMergeEffectBase> GetRequsted;
        public event Action<SuikaMergeEffectBase> ReturnRequseted;

        public SuikaMergeEffectBase Get(MonsterGrade grade, Vector3 pos)
        {
            return GetRequsted?.Invoke(grade, pos);
        }

        public void Return(SuikaMergeEffectBase effect)
        {
            if (ReturnRequseted is null)
            {
                Destroy(effect.gameObject);
            }
            else
            {
                ReturnRequseted.Invoke(effect);
            }
        }
    }
}