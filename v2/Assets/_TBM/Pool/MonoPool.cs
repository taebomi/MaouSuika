using System;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace TBM.Pool
{
    /// <summary>
    /// get 시 직접 객체 활성화 필요, release 시 자동 객체 비활성화
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MonoPool<T> : IDisposable where T : MonoBehaviour
    {
        private readonly ObjectPool<T> _pool;
        private readonly T _prefab;
        private readonly Transform _container;
        private readonly Action<T> _onCreated;
        private readonly Action<T> _onReleased;

        public MonoPool(T prefab, Transform container, int prewarmCount, Action<T> onCreated = null,
            Action<T> onReleased = null)
        {
            _prefab = prefab;
            _container = container;
            _onCreated = onCreated;
            _onReleased = onReleased;
            _pool = new ObjectPool<T>(
                createFunc: Create,
                actionOnGet: null,
                actionOnRelease: OnInstanceReleased,
                actionOnDestroy: OnInstanceDestroyed,
                defaultCapacity: prewarmCount);
            Prewarm(prewarmCount);
        }

        protected virtual T Create()
        {
            var item = Object.Instantiate(_prefab, _container);
            _onCreated?.Invoke(item);
            return item;
        }

        private void OnInstanceReleased(T item)
        {
            _onReleased?.Invoke(item);
            item.gameObject.SetActive(false);
            item.transform.SetParent(_container, false);
        }

        private void OnInstanceDestroyed(T item)
        {
            if (!Application.isPlaying || item == null) return;
            Object.Destroy(item);
        }

        private void Prewarm(int count)
        {
            var buffer = new T[count];
            for (var i = 0; i < count; i++) buffer[i] = _pool.Get();
            for (var i = 0; i < count; i++) _pool.Release(buffer[i]);
        }

        public T Get() => _pool.Get();
        public void Release(T item) => _pool.Release(item);

        public void Dispose()
        {
            _pool.Dispose();
        }
    }
}