namespace MaouSuika.Gameplay
{
    public interface ISkillInputHandler
    {
        SkillInputResult HandleInput(in SkillInputSnapshot input);
        void EndInput();
    }
}