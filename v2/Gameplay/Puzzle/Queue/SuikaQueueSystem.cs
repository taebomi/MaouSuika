using System;
using System.Collections.Generic;
using TBM.MaouSuika.Core;
using TBM.MaouSuika.Data;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    public class SuikaQueueSystem : MonoBehaviour
    {
        [SerializeField] private SuikaQueueView view;

        private SuikaDeck _suikaDeck;
        private SuikaQueue _queue;

        private int _visibleQueueSize;

        private void Awake()
        {
            _suikaDeck = new SuikaDeck(GameRule.Puzzle.Queue.MIN, GameRule.Puzzle.Queue.MAX);
            _queue = new SuikaQueue(_suikaDeck, GameRule.Puzzle.Queue.SIZE);
        }

        private void OnEnable()
        {
            _queue.QueueChanged += RefreshUI;
        }

        public void Initialize(SuikaTierDataTable tierDataTable)
        {
            view.Initialize(tierDataTable);
        }

        public void Setup(int visibleQueueSize)
        {
            _visibleQueueSize = visibleQueueSize;

            _queue.Setup();
            view.Refresh(_queue.Items, _visibleQueueSize);
        }

        private void OnDisable()
        {
            if (_queue != null)
            {
                _queue.QueueChanged -= RefreshUI;
            }
        }

        public int Dequeue()
        {
            return _queue.Dequeue();
        }

        public void ChangeVisibleQueueSize(int size)
        {
            _visibleQueueSize = size;
            RefreshUI(_queue.Items);
        }

        private void RefreshUI(IReadOnlyList<int> items)
        {
            view.Refresh(items, _visibleQueueSize);
        }
    }
}