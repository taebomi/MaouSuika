using System.Collections.Generic;
using UnityEngine;

namespace TBM.Core.Coroutines
{
    public static class YieldCache
    {
        public static WaitForFixedUpdate WaitForFixedUpdate;
        public static WaitForEndOfFrame WaitForEndOfFrame;

        private static Dictionary<float, WaitForSeconds> _waitForSecondsDict;

        private class FloatComparer : IEqualityComparer<float>
        {
            public bool Equals(float x, float y)
            {
                return Mathf.Approximately(x, y);
            }

            public int GetHashCode(float obj)
            {
                return obj.GetHashCode();
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void OnSubsystemRegistration()
        {
            WaitForFixedUpdate = new WaitForFixedUpdate();
            WaitForEndOfFrame = new WaitForEndOfFrame();

            _waitForSecondsDict = new Dictionary<float, WaitForSeconds>(new FloatComparer());
        }

        public static WaitForSeconds WaitForSeconds(float seconds)
        {
            if (_waitForSecondsDict.TryGetValue(seconds, out var waitForSeconds))
            {
                return waitForSeconds;
            }

            waitForSeconds = new WaitForSeconds(seconds);
            _waitForSecondsDict.Add(seconds, waitForSeconds);
            return waitForSeconds;
        }
    }
}