using UnityEngine;

namespace TBM.Extensions
{
    public static class MonoBehaviourExtensions
    {
        public static void SetActive(this MonoBehaviour monoBehaviour, bool active)
        {
            monoBehaviour.gameObject.SetActive(active);
        }

        public static void StopCoroutineSafe(this MonoBehaviour monoBehaviour, Coroutine coroutine)
        {
            if (coroutine == null) return;

            monoBehaviour.StopCoroutine(coroutine);
        }
    }
}