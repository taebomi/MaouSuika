using System;
using System.Collections.Generic;
using UnityEngine;

namespace SOSG.Stage.Suika.Combo
{
    public class ComboEffectPool : MonoBehaviour
    {
        [SerializeField] private ComboEffectPoolSO poolSO;

        [SerializeField] private ComboEffect prefab;

        private Stack<ComboEffect> _pool;

        private void Awake()
        {
            _pool = new Stack<ComboEffect>();
        }

        private void OnEnable()
        {
            poolSO.GetRequested += OnGetRequested;
            poolSO.ReturnRequested += OnReturnRequested;
        }

        private void OnDisable()
        {
            poolSO.GetRequested -= OnGetRequested;
            poolSO.ReturnRequested -= OnReturnRequested;
        }

        private ComboEffect OnGetRequested(Vector3 pos)
        {
            if (_pool.TryPop(out var comboEffect) is false)
            {
                comboEffect = Instantiate(prefab, pos, Quaternion.identity, transform);
            }
            else
            {
                comboEffect.transform.position = pos;
            }

            return comboEffect;
        }

        private void OnReturnRequested(ComboEffect comboEffect)
        {
            _pool.Push(comboEffect);
        }
    }
}