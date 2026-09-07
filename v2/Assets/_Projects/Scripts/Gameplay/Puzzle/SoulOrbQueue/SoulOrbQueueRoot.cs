using System;
using MaouSuika.Gameplay;
using R3;
using UnityEngine;

namespace MaouSuika.Gameplay
{
    public class SoulOrbQueueRoot : MonoBehaviour
    {
        [SerializeField] private SoulOrbTierDefinitionSO tierDefinition;

        [SerializeField] private SoulOrbQueueView queueView;

        private MonsterLoadout _monsterLoadout;

        private SoulOrbDeck _deck;
        private SoulOrbQueue _queue;
        private SoulOrbQueueViewModel _queueViewModel;

        public void Initialize(MonsterLoadout monsterLoadout)
        {
            _monsterLoadout = monsterLoadout;
        }

        public void Setup(int seed)
        {
            Dispose();

            _deck = new SoulOrbDeck(seed, tierDefinition.DrawableTierCount);
            _queue = new SoulOrbQueue(_deck.Draw);
            _queueViewModel = new SoulOrbQueueViewModel(_queue, _monsterLoadout);

            queueView.Setup(_queueViewModel);
        }

        private void OnDestroy()
        {
            Dispose();
        }

        public int Dequeue() => _queue.Dequeue();

        private void Dispose()
        {
            _queueViewModel?.Dispose();
            _queue?.Dispose();

            _deck = null;
            _queueViewModel = null;
            _queue = null;
        }
    }
}