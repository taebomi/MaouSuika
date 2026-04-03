using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Battle
{
    [CreateAssetMenu(fileName = "RandomHeroPartySO", menuName = "Maou Suika/Battle/Party/Random")]
    public class RandomHeroPartySO : HeroPartySO
    {
        [SerializeField] private HeroPartyComposition[] compositions;

        public override HeroObject[] ResolveMembers()
        {
            var totalWeight = 0f;
            foreach (var c in compositions)
                totalWeight += c.weight;

            var roll = Random.value * totalWeight;
            var cumulative = 0f;
            foreach (var c in compositions)
            {
                cumulative += c.weight;
                if (roll <= cumulative)
                    return c.members;
            }

            return compositions[^1].members;
        }
    }
}
