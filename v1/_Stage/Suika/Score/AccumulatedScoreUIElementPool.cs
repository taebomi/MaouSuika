using System;
using System.Collections.Generic;
using UnityEngine;

namespace SOSG.Stage.Suika.Score
{
    public class AccumulatedScoreUIElementPool : MonoBehaviour
    {
        [SerializeField] private AccumulatedScoreUIElementPoolSO poolSO;

        [SerializeField] private AccumulatedScoreUIElement prefab;

        private Stack<AccumulatedScoreUIElement> _pool;

        private void Awake()
        {
            _pool = new Stack<AccumulatedScoreUIElement>();
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

        private AccumulatedScoreUIElement OnGetRequested(Transform container)
        {
            if (_pool.TryPop(out var element) is false)
            {
                element = Instantiate(prefab, container);
            }
            else
            {
                element.transform.SetParent(container);
            }

            return element;
        }

        private void OnReturnRequested(AccumulatedScoreUIElement element)
        {
            element.transform.SetParent(transform);
            _pool.Push(element);
        }
    }
}