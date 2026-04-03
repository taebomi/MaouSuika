using System.Collections.Generic;
using UnityEngine;

namespace SOSG.Stage.Suika
{
    public static class SuikaUtility
    {
        public const int MinTier = 0;
        public const int ShootableMaxTier = 4;
        public const int MaxTier = 10;

        public static int GetRandomTier() => Random.Range(MinTier, MaxTier + 1);
        public static int GetShootableRandomTier() => Random.Range(MinTier, ShootableMaxTier + 1);

        public static readonly float[] SizeArr = { 0.5f, 0.75f, 1.25f, 2f, 2.625f, 3f, 3.5f, 4f, 4.5f, 5f, 5.25f };
    }
}