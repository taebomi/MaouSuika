using System;
using TBM.Extensions;
using UnityEngine;
using UnityEngine.Pool;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    public class ComboFeedbackController : MonoBehaviour
    {
        [SerializeField] private ComboEffect comboEffectPrefab;

        private IObjectPool<ComboEffect> _pool;
        
        public void Initialize()
        {
            _pool = new ObjectPool<ComboEffect>(CreateEffect,
                actionOnGet: effect => effect.gameObject.SetActive(true),
                actionOnRelease: effect => effect.gameObject.SetActive(false)
            );
        }

        public void HandleComboTriggered(ComboEvent e)
        {
            var effect = _pool.Get();
            effect.transform.position = e.Position;
            effect.Setup(e.Combo);
        }

        private ComboEffect CreateEffect()
        {
            var effect = Instantiate(comboEffectPrefab, transform);
            effect.Initialize(_pool);
            return effect;
        }
    }
}