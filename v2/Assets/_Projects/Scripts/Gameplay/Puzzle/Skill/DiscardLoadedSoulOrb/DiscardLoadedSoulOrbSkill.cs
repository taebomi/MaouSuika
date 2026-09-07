using UnityEngine;

namespace MaouSuika.Gameplay
{
    public class DiscardLoadedSoulOrbSkill : ISkill
    {
        private readonly SoulOrbRoot _soulOrbRoot;
        private readonly ShooterRoot _shooterRoot;

        public SkillDefinitionSO Definition { get; }

        public DiscardLoadedSoulOrbSkill(SkillDefinitionSO definition, SoulOrbRoot soulOrbRoot, ShooterRoot shooterRoot)
        {
            Definition = definition;
            _soulOrbRoot = soulOrbRoot;
            _shooterRoot = shooterRoot;
        }

        public SkillUseResult TryUse()
        {
            if (!_shooterRoot.TryDiscardLoadedOrb(out var discardedOrbSnapshot))
            {
                return SkillUseResult.None;
            }

            _soulOrbRoot.PlayPopEffect(in discardedOrbSnapshot, 0);
            return SkillUseResult.Executed;
        }
    }
}