using SOSG.Stage.Suika.Combo;
using TaeBoMi.Pool;
using TMPro;
using UnityEngine;

namespace SOSG.Stage
{
    public class ComboEffect : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;
        [SerializeField] private float fontSize;

        [SerializeField] private Color[] comboColor;

        // ReSharper disable once InconsistentNaming
        [SerializeField] private float aniVar_fontSizeMultiplier;

        private IObjectPool<ComboEffect> _pool;

        private const int ComboFirstGrade = (int)ComboGrade.Low;
        private const int ComboLastGrade = (int)ComboGrade.Extreme;

        private const float MinFontSize = 10f;
        private const float MaxFontSize = 15f;

        public void Initialize(IObjectPool<ComboEffect> pool)
        {
            _pool = pool;
        }

        public void Set(Vector3 pos, int combo)
        {
            var range = Mathf.Clamp(combo, ComboFirstGrade, ComboLastGrade - 1);
            var ratio = (float)(range - ComboFirstGrade) / (ComboLastGrade - 1 - ComboFirstGrade);

            text.color = comboColor[range - ComboFirstGrade];
            fontSize = Mathf.Lerp(MinFontSize, MaxFontSize, ratio);
            text.text = combo switch
            {
                >= (int)ComboGrade.Extreme => $"<shake a=0.15 d=0.5><rainb f=2 w=0.5>x{combo}</rainb></shake>",
                >= (int)ComboGrade.High => $"<bounce a=0.05>x{combo}</bounce>",
                >= (int)ComboGrade.Mid => $"<wave a=0.05 f=2>x{combo}</wave>",
                _ => $"x{combo}",
            };

            transform.position = pos;
            gameObject.SetActive(true);
        }

        private void Update()
        {
            text.fontSize = fontSize * aniVar_fontSizeMultiplier;
        }

        public void AniEvent_OnFinished()
        {
            gameObject.SetActive(false);
            _pool.Release(this);
        }
    }
}