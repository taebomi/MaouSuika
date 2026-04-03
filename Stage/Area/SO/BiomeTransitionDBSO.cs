using AYellowpaper.SerializedCollections;
using SOSG.Stage.Map;
using UnityEngine;

namespace SOSG.Stage.Area
{
    [CreateAssetMenu(fileName = "BiomeTransitionDBSO", menuName = "TaeBoMi/Stage/Map/BiomeTransitionDB", order = 9000)]
    public class BiomeTransitionDBSO : ScriptableObject
    {
        [SerializeField] private SerializedDictionary<BiomeType, BattleArea> data;

        public BattleArea this[BiomeType biomeType] => data[biomeType];
    }
}