using System.Collections.Generic;
using UnityEngine;

namespace SOSG.Stage.Suika
{
    [CreateAssetMenu(menuName = "SOSG/Suika/Collider Dict", fileName = "SuikaColliderDictSO")]
    public class SuikaColliderDictSO : ScriptableObject
    {
        private readonly Dictionary<int, SuikaObject> _dict = new();

        public SuikaObject this[int key]
        {
            get => _dict[key];
            set => _dict[key] = value;
        }

        public void SetUp()
        {
            _dict.Clear();
        }

        public void Add(int key, SuikaObject value)
        {
            _dict.Add(key, value);
        }

        public void Remove(int key)
        {
            _dict.Remove(key);
        }

        public bool TryGetValue(int key, out SuikaObject value) => _dict.TryGetValue(key, out value);
    }
}