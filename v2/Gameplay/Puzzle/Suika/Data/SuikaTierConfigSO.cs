using System;
using System.Collections.Generic;
using TBM.MaouSuika.Core.Vibration;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    /// <summary>
    /// 티어별 Suika 규칙 정의
    /// </summary>
    [CreateAssetMenu(fileName = "SuikaTierConfigSO", menuName = "Maou Suika/Puzzle/Suika/Tier Config")]
    public class SuikaTierConfigSO : ScriptableObject
    {
        [SerializeField] private List<SuikaTierDefinition> tierDefinitions;

        public int Count => tierDefinitions.Count;

        public SuikaTierDefinition this[int tier]
        {
            get
            {
                if (tier < 0 || tier >= tierDefinitions.Count)
                {
                    Logger.Error($"Tier[{tier}] is out of range");
                    return tierDefinitions[0];
                }

                return tierDefinitions[tier];
            }
        }
    }

    /// <summary>
    /// Suika 규칙 정의
    /// </summary>
    [Serializable]
    public struct SuikaTierDefinition
    {
        public float size;
        public float mass;
        public int score;

        public VibrationType collisionHaptic;
    }
}