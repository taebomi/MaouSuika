using System.Collections.Generic;
using UnityEngine;

namespace SOSG.Stage.Suika
{
    [CreateAssetMenu(menuName = "SOSG/Suika/Linked List", fileName = "SuikaLinkedListSO")]
    public class SuikaLinkedListSO : ScriptableObject
    {
        private readonly LinkedList<SuikaObject> _list = new();

#if UNITY_EDITOR
        [TextArea] [SerializeField] private string memo;
#endif

        public void SetUp()
        {
            _list.Clear();
        }

        public void AddFirst(SuikaObject suikaObject)
        {
            _list.AddFirst(suikaObject);
        }

        public void AddLast(SuikaObject suikaObject)
        {
            _list.AddLast(suikaObject);
        }

        public bool Remove(SuikaObject suikaObject)
        {
            return _list.Remove(suikaObject);
        }
    }
}