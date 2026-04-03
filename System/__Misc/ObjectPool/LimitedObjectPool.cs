using System;
using System.Collections.Generic;
using UnityEngine;

namespace TaeBoMi.Pool
{
    public class LimitedObjectPool<T> : IObjectPool<T> where T : MonoBehaviour
    {
        private readonly Stack<T> _pool;
        private readonly LinkedList<T> _usingList;
        private readonly Func<T> _createFunc;

        private readonly int _maxNum;
    
        public int UsingCount => _usingList.Count;
    
        public LimitedObjectPool(Func<T> createFunc, int maxNum)
        {
            _pool = new Stack<T>(maxNum);
            _usingList = new LinkedList<T>();
            _createFunc = createFunc;
            _maxNum = maxNum;
        }

        public T Get()
        {
            if (_pool.TryPop(out var pooled))
            {
                _usingList.AddLast(pooled);
                return pooled;
            }

            if (_usingList.Count >= _maxNum)
            {
                pooled = _usingList.First.Value;
                _usingList.RemoveFirst();
                _usingList.AddLast(pooled);
                return pooled;
            }

            pooled = _createFunc();
            _usingList.AddLast(pooled);
            return pooled;
        }
    
        public void Release(T willPooled)
        {
            _pool.Push(willPooled);
            _usingList.Remove(willPooled);
        }

        public void Clear()
        {
            throw new Exception("풀링 Clear 미구현");
        }
    }
}