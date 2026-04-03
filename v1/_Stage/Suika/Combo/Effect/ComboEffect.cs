using System;
using System.Net.Mime;
using SOSG.Stage;
using TMPro;
using UnityEngine;

namespace SOSG.Stage.Suika.Combo
{
    public class ComboEffect : MonoBehaviour
    {
        private const int ComboFirstGrade = (int)ComboGrade.Low;
        private const int ComboLastGrade = (int)ComboGrade.Extreme;

        private const float MinFontSize = 10f;
        private const float MaxFontSize = 15f;

        [SerializeField] private ComboEffectPoolSO poolSO;
            
        [SerializeField] private TMP_Text tmp;
        [SerializeField] private float fontSize;
        [SerializeField] private Color[] comboColor;
        [SerializeField] private float aniVar_fontSizeMultiplier;

        private void Update()
        {
            tmp.fontSize = fontSize * aniVar_fontSizeMultiplier;
        }

        public void AniEvent_OnFinished()
        {
            gameObject.SetActive(false);
            poolSO.Return(this);
        }

        public void Set(int combo)
        {
            var clamped = Mathf.Clamp(combo, ComboFirstGrade, ComboLastGrade - 1);
            var ratio = (float)(clamped - ComboFirstGrade) / (ComboLastGrade - 1 - ComboFirstGrade);

            fontSize = Mathf.Lerp(MinFontSize, MaxFontSize, ratio);
            tmp.color = comboColor[clamped - ComboFirstGrade];
            tmp.text = combo switch
            {
                >= (int)ComboGrade.Extreme => $"<shake a=0.15 d=0.5><rainb f=2 w=0.5>x{combo}</rainb></shake>",
                >= (int)ComboGrade.High => $"<bounce a=0.05>x{combo}</bounce>",
                >= (int)ComboGrade.Mid => $"<wave a=0.05 f=2>x{combo}</wave>",
                _ => $"x{combo}",
            };

            gameObject.SetActive(true);
        }
    }
}       