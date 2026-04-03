using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Battle
{
    public abstract class HeroPartySO : ScriptableObject
    {
        public abstract HeroObject[] ResolveMembers();
    }
}
