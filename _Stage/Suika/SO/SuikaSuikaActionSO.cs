using System;
using UnityEngine;

namespace SOSG.Stage.Suika
{
    [CreateAssetMenu(menuName = "SOSG/Suika/SuikaSuikaAction", fileName = "SuikaSuikaActionSO")]
    public class SuikaSuikaActionSO : ScriptableObject
    {
        public event Action<SuikaObject, SuikaObject> EventRaised;

#if UNITY_EDITOR
        [TextArea] [SerializeField] private string memo;
#endif

        public void RemoveAllListeners()
        {
            EventRaised = null;
        }

        public void RaiseEvent(SuikaObject suika1, SuikaObject suika2)
        {
            EventRaised?.Invoke(suika1, suika2);
        }
    }
}