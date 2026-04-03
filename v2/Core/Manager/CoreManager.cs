using System;
using UnityEngine;

namespace TBM.MaouSuika.Core
{
    public abstract class CoreManager<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance { get; private set; }

        public static void ResetSingleton()
        {
            Instance = null;
        }

        public void RegisterSingleton()
        {
            if (Instance != null) throw new Exception($"Already initialized: {Instance.GetType().Name}");

            Instance = this as T;
        }
    }
}