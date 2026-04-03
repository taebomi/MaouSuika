using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    [CreateAssetMenu(fileName = "SuikaSlashConfig", menuName = "Maou Suika/Puzzle/Skill/SuikaSlash Config")]
    public class SuikaSlashConfigSO : ScriptableObject
    {
        [SerializeField] private float maxGauge = 100f;
        [SerializeField] private float overlayDarkDuration = 0.3f;
        [SerializeField] private float overlayBrightDuration = 0.3f;
        [SerializeField] private float slashEffectDuration = 0.5f;
        [SerializeField] private float cursorMoveSpeed = 8f;

        public float MaxGauge => maxGauge;
        public float OverlayDarkDuration => overlayDarkDuration;
        public float OverlayBrightDuration => overlayBrightDuration;
        public float SlashEffectDuration => slashEffectDuration;
        public float CursorMoveSpeed => cursorMoveSpeed;
    }
}
