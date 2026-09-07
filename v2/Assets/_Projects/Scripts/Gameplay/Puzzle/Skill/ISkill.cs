namespace MaouSuika.Gameplay
{
    public interface ISkill
    {
        SkillDefinitionSO Definition { get; }
        SkillUseResult TryUse();
    }
}