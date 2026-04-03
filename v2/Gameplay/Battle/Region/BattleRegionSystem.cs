using System.Collections;
using TBM.Core.Coroutines;
using TBM.MaouSuika.Gameplay;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Battle
{
    public partial class BattleRegionSystem : MonoBehaviour
    {
        [SerializeField] private RegionProgressionSO progression;
        [SerializeField] private RegionTransitionSO transition;

        private BattleRegion _currentRegion;
        private BattleRegion _currentRegionPrefab;
        private BattleRegion _nextRegion;

        private Coroutine _transitionRoutine;

        public void Setup()
        {
            if (_transitionRoutine != null)
            {
                StopCoroutine(_transitionRoutine);
                _transitionRoutine = null;
            }
            

            if (_currentRegion != null)
            {
                Destroy(_currentRegion.gameObject);
            }

            if (_nextRegion != null)
            {
                Destroy(_nextRegion.gameObject);
            }

            _currentRegionPrefab = progression.InitialBattle;
            _currentRegion = Instantiate(_currentRegionPrefab, transform);
            _currentRegion.transform.localPosition = Vector3.zero;
        }

        public void TransitionByScore(int score)
        {
            if (_transitionRoutine != null) return;

            var nextRegionPrefab = progression.GetBattleRegion(score);
            if (_currentRegionPrefab == nextRegionPrefab) return;

            _transitionRoutine = StartCoroutine(TransitionTo(nextRegionPrefab));
        }

        private IEnumerator TransitionTo(BattleRegion nextRegionPrefab)
        {
            _nextRegion = Instantiate(nextRegionPrefab, transform);
            _nextRegion.Prepare(transition.SlideOptions);

            var fadeOut = StartCoroutine(_currentRegion.FadeOutRoutine(transition.FadeOptions));
            var fadein = StartCoroutine(_nextRegion.FadeInRoutine(transition.FadeOptions));

            var slideOut = StartCoroutine(_currentRegion.SlideOutRoutine(transition.SlideOptions));
            yield return YieldCache.WaitForSeconds(transition.SlideDelay);
            var slideIn = StartCoroutine(_nextRegion.SlideInRoutine(transition.SlideOptions));

            yield return fadeOut;
            yield return fadein;
            yield return slideOut;
            yield return slideIn;

            Destroy(_currentRegion.gameObject);
            _nextRegion.Finish();

            _currentRegion = _nextRegion;
            _currentRegionPrefab = nextRegionPrefab;
            _nextRegion = null;

            _transitionRoutine = null;
        }
    }
}