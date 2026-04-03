using System;
using UnityEngine;

namespace SOSG.Stage.Suika.Score
{
    [CreateAssetMenu(fileName = "AccumulatedScoreUIElementPoolSO",
        menuName = "SOSG/Suika/Score/Accumulated Score UI Element Pool")]
    public class AccumulatedScoreUIElementPoolSO : ScriptableObject
    {
        public event Func<Transform, AccumulatedScoreUIElement> GetRequested;
        public event Action<AccumulatedScoreUIElement> ReturnRequested;

        public AccumulatedScoreUIElement Get(Transform container)
        {
            return GetRequested?.Invoke(container);
        }

        public void Return(AccumulatedScoreUIElement element)
        {
            if (ReturnRequested is null)
            {
                Destroy(element.gameObject);
            }
            else
            {
                ReturnRequested.Invoke(element);
            }
        }
    }
}