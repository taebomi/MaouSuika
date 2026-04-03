using System;
using CodeStage.AntiCheat.ObscuredTypes;
using SOSG.Stage.Suika.Combo;
using SOSG.Stage.Suika.Shooter;
using UnityEngine;

namespace SOSG.Stage.Suika.Score
{
    public class ScoreSystem : MonoBehaviour
    {
        [SerializeField] private SuikaShooter suikaShooter;
        [SerializeField] private SuikaMerger suikaMerger;
        [SerializeField] private Combo.ComboSystem comboSystem;

        [SerializeField] private ScoreSystemUI ui;

        [SerializeField] private ScoreDataSO scoreDataSO;

        public ObscuredInt _curScore;
        private float _scoreMultiplier;

        public event Action<int> ScoreGot;

        private void Awake()
        {
            _curScore = 0;
        }

        private void OnEnable()
        {
            suikaShooter.Shot += OnShot;
            suikaMerger.Merged += OnSuikaMerged;
        }

        private void OnDisable()
        {
            suikaShooter.Shot -= OnShot;
            suikaMerger.Merged -= OnSuikaMerged;
        }

        private void OnSuikaMerged(SuikaMerger.MergedInfo info)
        {
            comboSystem.OnMerged(info);
            
            var score = scoreDataSO.GetScore(info.Tier, comboSystem.CurComboGrade);
            _curScore += score;
            ScoreGot?.Invoke(score);
        }

        private void OnShot()
        {
            comboSystem.OnShot();
        }
    }
}