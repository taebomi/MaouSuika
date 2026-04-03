using System;
using System.Collections.Generic;
using UnityEngine;

namespace SOSG.Stage.Suika
{
    public class SuikaQueue : MonoBehaviour
    {
        [SerializeField] private SuikaQueueManager queueManager;

        private Queue<int> _queue = new(100);

        private void Awake()
        {
            _queue.Clear();
        }

        public int Pop()
        {
            if (_queue.Count == 0)
            {
                queueManager.EnqueueAll();
            }

            return _queue.Dequeue();
        }

        public void Push(int tier)
        {
            _queue.Enqueue(tier);
        }
    }
}