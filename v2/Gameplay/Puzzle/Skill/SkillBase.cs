using System.Collections;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    public abstract class SkillBase : MonoBehaviour, ISkill
    {
        public abstract float MaxGauge { get; }
        public bool WasCancelled { get; protected set; }

        public abstract IEnumerator Execute(SkillExecutionContext context);
    }
}
