using System;
using System.Collections.Generic;
using SOSG.Stage.Suika.Combo;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;

namespace SOSG.Stage.Suika.Score
{
    public class ScoreSystemUI : MonoBehaviour
    {
        [SerializeField] private ScoreSystem scoreSystem;
        [SerializeField] private Combo.ComboSystem comboSystem;

        [SerializeField] private TMP_Text bestScoreTmp;
        [SerializeField] private CurrentScoreUI curScoreUI;


        private void OnEnable()
        {
            scoreSystem.ScoreGot += OnScoreGot;
            comboSystem.ComboFailed += OnComboFailed;
        }

        private void OnDisable()
        {
            scoreSystem.ScoreGot -= OnScoreGot;
            comboSystem.ComboFailed -= OnComboFailed;
        }

        private void OnScoreGot(int score)
        {
            curScoreUI.AddScore(score);
        }

        private void OnComboFailed()
        {
            curScoreUI.StopAccumulating();
        }
    }
}