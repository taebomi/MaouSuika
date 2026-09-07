using System;
using MaouSuika.Core;
using UnityEngine;

namespace MaouSuika.Gameplay
{
    [CreateAssetMenu(fileName = "SoulOrb_ShooterSettings_SO"
        , menuName = MenuPath.Gameplay.SHOOTER + "Settings")]
    public class SoulOrbShooterSettingsSO : ScriptableObject
    {
        [Serializable]
        public class FireSettings
        {
            [field: SerializeField] public float ShootPower { get; private set; } = 20f;
            [field: SerializeField] public float FireCooldown { get; private set; } = 0.25f;
        }

        [Serializable]
        public class LoadedSoulOrbStyle
        {
            [field: SerializeField] public float SpinSpeed { get; private set; } = 30f;
            [field: SerializeField] public float RiseOffset { get; private set; } = 1.5f;
            [field: SerializeField] public float PopStartScale { get; private set; } = 0.35f;
            [field: SerializeField] public float PopOvershoot { get; private set; } = 3;
        }

        [Serializable]
        public class AimStyle
        {
            [field: SerializeField] public Gradient Gradient { get; private set; }
            /// <summary>sprite 9-slice 세로 높이. 그냥 스프라이트에 맞게 대충 세팅해둔 값임.
            /// 나중에 정밀한 값 필요 시, 여기가 아니라 동적으로 세팅 필요함.</summary>
            [field: SerializeField] public float YSize { get; private set; } = 2f;
            [field: SerializeField] public float MinXSize { get; private set; } = 1.5f;
            [field: SerializeField] public float MaxXSize { get; private set; } = 5f;
        }


        [field: SerializeField] public FireSettings Fire { get; private set; }
        [field: SerializeField] public LoadedSoulOrbStyle LoadedOrb { get; private set; }
        [field: SerializeField] public AimStyle Aim { get; private set; }

        [SerializeField] private Color blockedColor = new Color(1f, 0.35f, 0.35f);
        [SerializeField] private float cooldownAlpha = 0.5f;

        public AimVisualData EvaluateAim(AimData aimData, ShooterVisualState state)
        {
            var size = new Vector2(Aim.MinXSize + (Aim.MaxXSize - Aim.MinXSize) * aimData.PowerRatio, Aim.YSize);
            var color = GetColor(aimData.PowerRatio, state);
            return new AimVisualData(size, color);
        }

        private Color GetColor(float powerRatio, ShooterVisualState state)
        {
            switch (state)
            {
                case ShooterVisualState.Blocked:
                    return blockedColor;
                case ShooterVisualState.Cooldown:
                    var color = Aim.Gradient.Evaluate(powerRatio);
                    color.a = cooldownAlpha;
                    return color;
                case ShooterVisualState.Ready:
                    return Aim.Gradient.Evaluate(powerRatio);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public Color EvaluateLoadedOrbTint(ShooterVisualState state) =>
            state is ShooterVisualState.Blocked ? blockedColor : Color.white;
    }
}