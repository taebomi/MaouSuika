using UnityEngine;

namespace MaouSuika.Gameplay
{
    public readonly struct PuzzleInputSnapshot
    {
        public readonly ShooterInputSnapshot Shooter;
        public readonly bool UseSkillPressedThisFrame;

        public PuzzleInputSnapshot(ShooterInputSnapshot shooter, bool useSkillPressedThisFrame)
        {
            Shooter = shooter;
            UseSkillPressedThisFrame = useSkillPressedThisFrame;
        }
    }

    public readonly struct ShooterInputSnapshot
    {
        public readonly Vector2? Aim;
        public readonly bool FirePressedThisFrame;
        public readonly bool IsFirePressed;
        public readonly bool FireReleasedThisFrame;

        public ShooterInputSnapshot(Vector2? aim, bool firePressedThisFrame, bool isFirePressed,
            bool fireReleasedThisFrame)
        {
            Aim = aim;
            FirePressedThisFrame = firePressedThisFrame;
            IsFirePressed = isFirePressed;
            FireReleasedThisFrame = fireReleasedThisFrame;
        }
    }

    public readonly struct SkillInputSnapshot
    {
        public readonly Vector2? Point;
        public readonly Vector2 Navigate;
        public readonly bool ConfirmPressedThisFrame;
        public readonly bool CancelPressedThisFrame;

        public SkillInputSnapshot(Vector2? point, Vector2 navigate, bool confirmPressedThisFrame,
            bool cancelPressedThisFrame)
        {
            Point = point;
            Navigate = navigate;
            ConfirmPressedThisFrame = confirmPressedThisFrame;
            CancelPressedThisFrame = cancelPressedThisFrame;
        }
    }
}