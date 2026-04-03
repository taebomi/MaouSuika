using System;
using FMODUnity;
using UnityEngine.AddressableAssets;

namespace SOSG.Area
{
    [Serializable]
    public class AreaData
    {
        public string areaName;
        public float globalLightIntensity;

        public EventReference bgmRef;

        public AssetReference battleAreaRef;
        public AssetReference gashaponAreaRef;
    }
}