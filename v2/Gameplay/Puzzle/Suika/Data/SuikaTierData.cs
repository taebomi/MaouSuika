using System;
using TBM.MaouSuika.Core.Vibration;
using TBM.MaouSuika.Data;
using TBM.MaouSuika.Gameplay.Monster;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    /// <summary>
    /// 런타임 시 SuikaObject가 참조할 데이터
    /// </summary>
    public class SuikaTierData
    {
        public readonly int Tier;
        public readonly float Size;
        public readonly float Mass;
        public readonly int Score;
        public readonly VibrationType CollisionHaptic;
        public readonly MonsterDataSO MonsterData;

        public Color Color => MonsterData.suikaColor;
        public MergeEffectColor MergeEffectColor => MonsterData.mergeEffectColor;

        public SuikaTierData(int tier, SuikaTierDefinition def, MonsterDataSO monsterData)
        {
            Tier = tier;

            Size = def.size;
            Mass = def.mass;
            Score = def.score;

            CollisionHaptic = def.collisionHaptic;

            MonsterData = monsterData;
        }
    }

    public static class SuikaTierDataBuilder
    {
        public static SuikaTierData[] Build(SuikaTierConfigSO config, MonsterLoadout loadout)
        {
            if (config == null || loadout == null)
            {
                throw new ArgumentNullException();
            }

            if (config.Count != loadout.Count)
            {
                Logger.Warning($"Config Count[{config.Count}] != Loadout Count[{loadout.Count}]");
            }

            var count = Mathf.Min(config.Count, loadout.Count);
            if (count == 0)
            {
                throw new InvalidOperationException($"Empty Data !");
            }

            var data = new SuikaTierData[count];
            for (var tier = 0; tier < count; tier++)
            {
                data[tier] = new SuikaTierData(tier, config[tier], loadout[tier]);
            }

            return data;
        }
    }
}