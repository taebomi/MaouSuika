using System.Collections;
using TBM.MaouSuika.Gameplay.Player;
using TBM.MaouSuika.Gameplay.Monster;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Battle
{
    public class BattleController : MonoBehaviour
    {
        [SerializeField] private BattleSide battleSide;
        [SerializeField] private BattleConfigSO battleConfig;

        [SerializeField] private MonsterSystem monsterSystem;
        [SerializeField] private PartySystem partySystem;
        [SerializeField] private BattleRegionSystem regionSystem;

        private bool _isSequencePlaying;

        private int _currentScore;

        public void Initialize(PlayerContext context)
        {
            monsterSystem.Initialize(context.MonsterLoadout);
        }

        public void Setup()
        {
            regionSystem.Setup();
            // partySystem.SpawnNextParty(0);
            // partySystem.PartyDefeated += OnPartyDefeated;
        }

        public void Tick(float deltaTime) { }


        private void OnDisable()
        {
            // partySystem.PartyDefeated -= OnPartyDefeated;
        }

        private void OnPartyDefeated()
        {
            if (_isSequencePlaying) return;
            StartCoroutine(PartyDefeatedSequence());
        }

        public void OnScoreChanged(int score)
        {
            _currentScore = score;
            regionSystem.TransitionByScore(score);
        }

        private IEnumerator PartyDefeatedSequence()
        {
            _isSequencePlaying = true;

            // 1. 몬스터 승리 연출
            monsterSystem.PlayVictoryAll(MonsterVictoryType.SpinJump);
            yield return new WaitForSeconds(battleConfig.VictoryDisplayDuration);
            monsterSystem.StopVictoryAll();

            // 3. 다음 파티 등장
            partySystem.SpawnNextParty(_currentScore);

            _isSequencePlaying = false;
        }

        public IEnumerator PlayGameOverSequence()
        {
            Logger.Info("Battle 게임오버 연출");
            yield return null;
        }

        public void OnPuzzleMergeActionFinished(int tier)
        {
            monsterSystem.Spawn(tier, battleSide);
        }
    }
}