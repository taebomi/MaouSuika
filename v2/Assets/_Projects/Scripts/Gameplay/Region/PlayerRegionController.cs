using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using FMODUnity;
using R3;
using UnityEngine;

namespace MaouSuika.Gameplay
{
    public class PlayerRegionController : MonoBehaviour
    {
        [SerializeField] private RegionProgressionSO progression;
        [SerializeField] private PuzzleRegionController puzzleRegionController;

        private RegionDefinitionSO _requestedRegion;
        private bool _hasPendingTransition;
        private bool _isTransitioning;

        private Action<EventReference> _playBgm;

        public void Initialize(Action<EventReference> playBgm)
        {
            _playBgm = playBgm;
        }

        public void Setup(Observable<int> totalScoreChanged, int currentScore)
        {
            ApplyScore(currentScore);
            totalScoreChanged.Subscribe(ApplyScore).AddTo(this);
        }

        private void ApplyScore(int totalScore)
        {
            var requestedRegion = FindRegion(totalScore);
            if (_requestedRegion == requestedRegion) return;

            _requestedRegion = requestedRegion;
            _hasPendingTransition = true;

            if (!_isTransitioning) ProcessTransitionsAsync(destroyCancellationToken).Forget();
        }

        private async UniTask ProcessTransitionsAsync(CancellationToken token)
        {
            _isTransitioning = true;

            while (_hasPendingTransition)
            {
                var targetRegion = _requestedRegion;
                _hasPendingTransition = false;

                _playBgm(targetRegion.Bgm);
                await puzzleRegionController.TransitionToAsync(targetRegion.Puzzle, token);
            }

            _isTransitioning = false;
        }

        private RegionDefinitionSO FindRegion(int totalScore)
        {
            var entries = progression.Entries;
            var result = entries[0].Region;

            for (var i = 0; i < entries.Count; i++)
            {
                if (totalScore < entries[i].RequiredTotalScore) break;

                result = entries[i].Region;
            }

            return result;
        }
    }
}