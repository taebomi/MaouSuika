using System.Collections;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    public interface ISkill
    {
        float MaxGauge { get; }
        bool WasCancelled { get; }
        IEnumerator Execute(SkillExecutionContext context);
    }
}
