using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Battle
{
    [CreateAssetMenu(fileName = "FixedHeroPartySO", menuName = "Maou Suika/Battle/Party/Fixed")]
    public class FixedHeroPartySO : HeroPartySO
    {
        [SerializeField] private HeroObject[] members;

        public override HeroObject[] ResolveMembers() => members;
    }
}
