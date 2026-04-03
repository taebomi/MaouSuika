using System;
using FMODUnity;
using SOSG.Stage.Map;

namespace SOSG.Stage.Area
{
    [Serializable]
    public class AreaData
    {
        public string areaName;
        public BiomeType biomeType;
        
        public float globalLightIntensity;
        
        // Audio
        public EventReference bgmRef;
        
        // prefab
        public BattleArea battleAreaPrefab;
        public GashaponArea gashaponAreaPrefab;
    }
}