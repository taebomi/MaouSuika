using System;
using System.Collections.Generic;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Battle
{
    public class PartySystem : MonoBehaviour
    {
        [SerializeField] private Transform spawnRoot;
        [SerializeField] private HeroPartyPoolSO partyPool;

        public event Action PartyDefeated;

        private readonly List<HeroObject> _members = new();

        public void SpawnNextParty(int score)
        {
            var party = partyPool.SelectParty(score);
            foreach (var prefab in party.ResolveMembers())
            {
                var hero = Instantiate(prefab, spawnRoot);
                hero.Setup();
                hero.OnDied += OnHeroDied;
                _members.Add(hero);
            }

            ArrangeFormation();
        }

        private void OnHeroDied(HeroObject hero)
        {
            hero.OnDied -= OnHeroDied;
            _members.Remove(hero);

            if (_members.Count == 0)
            {
                PartyDefeated?.Invoke();
                return;
            }

            ArrangeFormation();
        }

        private void ArrangeFormation()
        {
            // 10번 BattleConfigSO에서 간격 값 적용 예정
            for (var i = 0; i < _members.Count; i++)
                _members[i].transform.localPosition = Vector3.zero;
        }
    }
}
