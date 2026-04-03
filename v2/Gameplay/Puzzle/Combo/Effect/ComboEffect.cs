using System;
using System.Collections;
using TBM.Core.Coroutines;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Serialization;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    public class ComboEffect : MonoBehaviour
    {
        [SerializeField] private TMP_Text tmp;


        [SerializeField] private float baseSize;
        [SerializeField] private float sizeMultiplier;

        private IObjectPool<ComboEffect> _pool;
        private Coroutine _lifeRoutine;

        public void Initialize(IObjectPool<ComboEffect> pool)
        {
            _pool = pool;
        }

        public void Setup(int combo)
        {
            var grade = ComboUtility.ConvertFrom(combo);


            tmp.text = grade switch
            {
                ComboGrade.Extreme => "",
                ComboGrade.High => "",
                ComboGrade.Mid => "",
                ComboGrade.Low => "",
                _ => $"x{combo}"
            };
            RestartLifeTimer(5f);
        }

        private void Update()
        {
            tmp.fontSize = baseSize * sizeMultiplier;
        }

        private void OnDisable()
        {
            if (_lifeRoutine != null)
            {
                StopCoroutine(_lifeRoutine);
                _lifeRoutine = null;
            }
        }


        private void SetTextEffect(int combo)
        {
        }


        // Pooling
        private void RestartLifeTimer(float duration)
        {
            if (_lifeRoutine != null) StopCoroutine(_lifeRoutine);

            _lifeRoutine = StartCoroutine(LifeRoutine());
        }

        private IEnumerator LifeRoutine()
        {
            yield return YieldCache.WaitForSeconds(5f);

            ReturnToPool();
        }

        private void ReturnToPool()
        {
            _lifeRoutine = null;

            if (_pool == null)
            {
                Destroy(gameObject);
                return;
            }

            gameObject.SetActive(false);
            _pool.Release(this);
        }
    }
}