using System;

namespace TBM.MaouSuika.Gameplay.Battle
{
    [Serializable]
    public struct HeroPartyComposition
    {
        public HeroObject[] members;
        public float weight;
    }
}
