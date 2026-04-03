
using UnityEngine;

namespace TaeBoMi.Pool
{
    public interface IObjectPool<T> where T : MonoBehaviour
    {
        T Get();
        void Release(T willPooled);
        void Clear();
    }
}