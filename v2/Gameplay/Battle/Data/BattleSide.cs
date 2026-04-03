using System;
using TBM.MaouSuika.Core;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Battle
{
    public enum BattleSide
    {
        Left,
        Right,
    }

    public static class BattleSideExtensions
    {
        public static Vector2 ToMoveDirection(this BattleSide side)
        {
            return side switch
            {
                BattleSide.Left => Vector2.right,
                BattleSide.Right => Vector2.left,
                _ => throw new ArgumentOutOfRangeException(nameof(side), side, null),
            };
        }

        public static int ToLayer(this BattleSide side)
        {
            return side switch
            {
                BattleSide.Left => Layers.BATTLE_SIDE_LEFT,
                BattleSide.Right => Layers.BATTLE_SIDE_RIGHT,
                _ => throw new ArgumentOutOfRangeException(nameof(side), side, null),
            };
        }
    }
}