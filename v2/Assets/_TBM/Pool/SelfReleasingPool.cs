using System;
using UnityEngine;

namespace TBM.Pool
{
    public class SelfReleasingPool<T> : MonoPool<T> where T : MonoBehaviour, ISelfReleasingPoolable
    {
        public SelfReleasingPool(T prefab, Transform container, int prewarmCount,
            Action<T> onCreated = null) : base(prefab, container, prewarmCount, onCreated) { }

        protected override T Create()
        {
            var instance = base.Create();
            instance.Initialize(() => Release(instance));
            return instance;
        }
    }
    
    public interface ISelfReleasingPoolable
    {
        void Initialize(Action onComplete);
    }
}