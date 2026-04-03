using System.Collections;
using TBM.MaouSuika.Gameplay;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    public partial class PuzzleRegionSystem : MonoBehaviour
    {
        [SerializeField] private RegionProgressionSO progression;
        [SerializeField] private PuzzleRegionTransitionSO transition;

        private PuzzleRegion _currentRegion;
        private PuzzleRegion _currentRegionPrefab;
        private PuzzleRegion _nextRegion;

        private Coroutine _transitionRoutine;

        public void Setup()
        {
            if (_transitionRoutine != null)
            {
                StopCoroutine(_transitionRoutine);
                _transitionRoutine = null;
            }

            if (_currentRegion != null)
                Destroy(_currentRegion.gameObject);

            if (_nextRegion != null)
                Destroy(_nextRegion.gameObject);

            _currentRegionPrefab = progression.InitialPuzzle;
            _currentRegion = Instantiate(_currentRegionPrefab, transform);
            _currentRegion.transform.localPosition = Vector3.zero;
        }

        public void TransitionByScore(int score)
        {
            if (_transitionRoutine != null) return;

            var nextRegionPrefab = progression.GetPuzzleRegion(score);
            if (_currentRegionPrefab == nextRegionPrefab) return;

            _transitionRoutine = StartCoroutine(TransitionTo(nextRegionPrefab));
        }

        private IEnumerator TransitionTo(PuzzleRegion nextRegionPrefab)
        {
            _nextRegion = Instantiate(nextRegionPrefab, transform);
            _nextRegion.Prepare();

            var fadeOut = StartCoroutine(_currentRegion.FadeOutRoutine(transition));
            var fadeIn = StartCoroutine(_nextRegion.FadeInRoutine(transition));

            yield return fadeOut;
            yield return fadeIn;

            Destroy(_currentRegion.gameObject);

            _currentRegion = _nextRegion;
            _currentRegionPrefab = nextRegionPrefab;
            _nextRegion = null;

            _transitionRoutine = null;
        }
    }
}
