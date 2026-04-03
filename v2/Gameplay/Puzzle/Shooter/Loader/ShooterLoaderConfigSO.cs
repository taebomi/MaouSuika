using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    [CreateAssetMenu(fileName = "Shooter Loader Config SO", menuName = "Maou Suika/Puzzle/Shooter/Loader Config")]
    public class ShooterLoaderConfigSO : ScriptableObject
    {
        [Header("Rotation")]
        [SerializeField] private float rotationSpeed = 90f;

        [Header("Color")]
        [SerializeField] private Color blockedColor = Color.red;
        [SerializeField] private float cooldownAlpha = 0.5f;

        public float RotationSpeed => rotationSpeed;

        public LoadedSuikaVisualData Evaluate(Color baseMonsterColor, ShooterState shooterState)
        {
            // Cooldown 우선순위
            if ((shooterState & ShooterState.Cooldown) != 0)
            {
                return new LoadedSuikaVisualData
                {
                    CoreColor = new Color(baseMonsterColor.r, baseMonsterColor.g, baseMonsterColor.b, cooldownAlpha),
                    DetailColor = new Color(1f, 1f, 1f, cooldownAlpha),
                };
            }

            // Block 우선순위
            if ((shooterState & ShooterState.Blocked) != 0)
            {
                return new LoadedSuikaVisualData
                {
                    CoreColor = blockedColor,
                    DetailColor = blockedColor,
                };
            }

            return new LoadedSuikaVisualData
            {
                CoreColor = baseMonsterColor,
                DetailColor = Color.white,
            };
        }
    }
}