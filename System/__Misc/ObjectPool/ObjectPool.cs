using System;
using System.Collections.Generic;
using UnityEngine;

namespace TaeBoMi.Pool
{
    public class ObjectPool<T> : IObjectPool<T> where T : MonoBehaviour
    {
        private readonly Stack<T> _pool;
        private readonly Func<T> _createFunc;

        public ObjectPool(Func<T> createFunc, int capacity = 10)
        {
            _pool = new Stack<T>(capacity);
            
            _createFunc = createFunc;
        }

        public T Get()
        {
            if (!_pool.TryPop(out var pooled))
            {
                pooled = _createFunc();
            }

            return pooled;
        }

        public void Release(T willPooled)
        {
            _pool.Push(willPooled);
        }

        public void Clear()
        {
            throw new NotImplementedException("풀링 Clear 미구현");
        }
    }
}