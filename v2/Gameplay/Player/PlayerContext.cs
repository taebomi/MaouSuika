using System;
using Sirenix.OdinInspector;
using TBM.MaouSuika.Core;
using TBM.MaouSuika.Data;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Player
{
    public class PlayerContext
    {
        [ShowInInspector, ReadOnly] public int PlayerIndex { get; }

        [ShowInInspector, ReadOnly] public MonsterLoadout MonsterLoadout { get;}

        public PlayerContext(int playerIndex, MonsterLoadout monsterLoadout)
        {
            PlayerIndex = playerIndex;
            MonsterLoadout = monsterLoadout;
        }
    }
}