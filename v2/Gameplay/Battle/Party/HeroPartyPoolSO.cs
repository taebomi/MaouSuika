using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Battle
{
    [CreateAssetMenu(fileName = "HeroPartyPoolSO", menuName = "Maou Suika/Battle/Party/Pool")]
    public class HeroPartyPoolSO : ScriptableObject
    {
        [SerializeField] private HeroPartyPoolEntry[] entries;

        public HeroPartySO SelectParty(int score)
        {
            var totalWeight = 0f;
            foreach (var e in entries)
                totalWeight += e.weightByScore.Evaluate(score);

            var roll = Random.value * totalWeight;
            var cumulative = 0f;
            foreach (var e in entries)
            {
                cumulative += e.weightByScore.Evaluate(score);
                if (roll <= cumulative)
                    return e.party;
            }

            return entries[^1].party;
        }
    }
}
