using System;
using System.Collections.Generic;
using SOSG.Monster;
using UnityEngine;

namespace SOSG.Stage.Suika.Merge
{
    public abstract class SuikaMergeEffectBase : MonoBehaviour
    {
        [SerializeField] private SuikaMergeEffectPoolSO poolSO;
        
        public abstract MonsterGrade Grade { get; }

        private void OnParticleSystemStopped()
        {
            gameObject.SetActive(false);
            poolSO.Return(this);
        }

        public abstract void SetSize(float size);

        public abstract void SetColor(MonsterDataSO.MergeEffectColor color);
    }
}