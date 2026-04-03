using System.Collections.Generic;
using UnityEngine;

namespace SOSG.Stage.Suika
{
    public class SuikaPool : MonoBehaviour
    {
        [SerializeField] private SuikaObject suikaPrefab;

        private Stack<SuikaObject> _pool;

        private void Awake()
        {
            _pool = new Stack<SuikaObject>(100);
        }

        public SuikaObject Pop(Vector3 pos)
        {
            if (_pool.TryPop(out var suika) is false)
            {
                suika = Instantiate(suikaPrefab, pos, Quaternion.identity, transform);
            }
            else
            {
                suika.SetPosition(pos);
            }
            return suika;
        }

        public void Push(SuikaObject suika)
        {
            _pool.Push(suika);
        }
    }
}