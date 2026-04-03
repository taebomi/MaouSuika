using System;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Battle
{
    [Serializable]
    public struct HeroPartyPoolEntry
    {
        public HeroPartySO party;
        public AnimationCurve weightByScore;
    }
}
