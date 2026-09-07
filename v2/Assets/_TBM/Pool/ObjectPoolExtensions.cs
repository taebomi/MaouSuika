using UnityEngine.Pool;

namespace TBM.Pool
{
    public static class ObjectPoolExtensions
    {
        public static void Prewarm<T>(this IObjectPool<T> pool, int count) where T : class
        {
            var buffer = new T[count];
            for (var i = 0; i < count; i++) buffer[i] = pool.Get();
            for (var i = 0; i < count; i++) pool.Release(buffer[i]);
        }
    }
}