namespace MaouSuika.Gameplay
{
    public class SkillLoadout
    {
        public SkillDefinitionSO EquippedSkill { get; }

        public SkillLoadout(SkillDefinitionSO definition)
        {
            EquippedSkill = definition;
        }
    }
}