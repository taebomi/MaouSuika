using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    public class MainScoreCounter : MonoBehaviour, IMainScoreCounter
    {
        [SerializeField] private TMP_Text scoreTmp;

        [SerializeField] private MainScoreEffectStrategySO effectStrategySO;

        [SerializeField] private Color defaultColor;
        [SerializeField] private string format = "{0:000000}";

        public Transform TargetTr => scoreTmp.transform;
        public TMP_Text TargetTmp => scoreTmp;
        public int DisplayScore => _displayScore;
        public Color BaseColor => defaultColor;

        private int _displayScore;

        private Sequence _sequence;
        
        private void OnDestroy()
        {
            _sequence?.Kill();
        }

        /// <summary>
        /// 연출과 함께 값 적용
        /// </summary>
        public void UpdateScore(int targetScore)
        {
            if (_sequence != null && _sequence.IsPlaying())
            {
                _sequence.Kill();
            }

            _sequence = effectStrategySO
                .CreateSequence(this, targetScore)
                .OnKill(() => _sequence = null)
                .Play();
        }

        public Color GetDefaultColor()
        {
            return defaultColor;
        }

        public int GetScore()
        {
            return _displayScore;
        }

        /// <summary>
        /// 값 즉시 적용
        /// </summary>
        public void SetScore(int score)
        {
            _displayScore = score;
            scoreTmp.SetText(format, score);
        }
    }
}