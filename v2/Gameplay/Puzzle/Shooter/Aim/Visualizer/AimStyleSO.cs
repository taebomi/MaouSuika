using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    [CreateAssetMenu(fileName = "Aim Style SO", menuName = "Maou Suika/Puzzle/Shooter/Aim/Style")]
    public class AimStyleSO : ScriptableObject
    {
        [SerializeField] private Gradient gradient;
        [SerializeField] private Color blockedColor;
        [SerializeField] private float cooldownAlpha;

        [SerializeField] private float ySize = 2f;
        [SerializeField] private float minXSize = 1.5f;
        [SerializeField] private float maxXSize = 5f;

        public AimVisualData Evaluate(AimData aimData, ShooterState state)
        {
            var length = minXSize + (maxXSize - minXSize) * aimData.PowerRatio;
            var size = new Vector2(length, ySize);
            var color = GetColor(aimData.PowerRatio, state);

            return new AimVisualData
            {
                Color = color,
                Size = size,
            };
        }

        private Color GetColor(float powerRatio, ShooterState state)
        {
            Color resultColor;
            if ((state & ShooterState.Cooldown) != 0)
            {
                resultColor = gradient.Evaluate(powerRatio);
                resultColor.a = cooldownAlpha;
                return resultColor;
            }

            if ((state & ShooterState.Blocked) != 0)
            {
                resultColor = blockedColor;
                resultColor.a = 1f;
                return resultColor;
            }

            resultColor = gradient.Evaluate(powerRatio);
            resultColor.a = 1f;
            return resultColor;
        }
    }
}