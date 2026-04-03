using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    public readonly struct PuzzleInputResult
    {
        public ShooterInputResult Shooter { get; init; }
        public SkillInputResult Skill { get; init; }

        public static PuzzleInputResult None => new();

        public override string ToString()
        {
            return $"[Shooter]\n{Shooter.ToString()}\n[Skill]\n{Skill.ToString()}";
        }
    }

    public readonly struct ShooterInputResult
    {
        public ShooterInputType InputType { get; init; }

        public Vector2 AimValue { get; init; }

        public bool AimStartedThisFrame { get; init; }
        public bool IsAiming { get; init; }
        public bool FireRequestedThisFrame { get; init; }

        public override string ToString()
        {
            return $"Input Type[{InputType}]\n" +
                   $"Aim Value[{AimValue}]\n" +
                   $"Aim Started[{AimStartedThisFrame}] / Is Aiming[{IsAiming}]\n" +
                   $"Fire Requested[{FireRequestedThisFrame}]";
        }
    }

    public enum SkillInputType
    {
        None,
        Pointer,
        VirtualCursor,
    }

    public readonly struct SkillInputResult
    {
        public bool SkillRequestedThisFrame { get; init; }

        public SkillInputType InputType { get; init; }

        public override string ToString()
        {
            return $"Input Type[{InputType}]\n" +
                   $"Skill Requested[{SkillRequestedThisFrame}]";
        }
    }
}