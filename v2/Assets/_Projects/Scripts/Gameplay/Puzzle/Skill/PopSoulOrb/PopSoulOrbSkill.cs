using System;

namespace MaouSuika.Gameplay
{
    public class PopSoulOrbSkill : ISkill, ISkillInputHandler
    {
        private readonly SingleSoulOrbTargeting _targeting;
        private readonly SoulOrbRoot _soulOrbRoot;
        private readonly ComboRoot _comboRoot;

        public SkillDefinitionSO Definition { get; }

        public PopSoulOrbSkill(
            SkillDefinitionSO definition, SingleSoulOrbTargeting targeting,
            SoulOrbRoot soulOrbRoot, ComboRoot comboRoot)
        {
            Definition = definition;
            _targeting = targeting;
            _soulOrbRoot = soulOrbRoot;
            _comboRoot = comboRoot;
        }

        public SkillUseResult TryUse()
        {
            _targeting.Begin();
            return SkillUseResult.AwaitingInput;
        }

        public SkillInputResult HandleInput(in SkillInputSnapshot input)
        {
            var result = _targeting.HandleInput(input);

            switch (result)
            {
                case TargetingResult.InProgress:
                    return SkillInputResult.InProgress;
                case TargetingResult.Confirmed:
                    return TryPopTarget();
                case TargetingResult.Canceled:
                    return SkillInputResult.Canceled;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void EndInput()
        {
            _targeting.End();
        }

        private SkillInputResult TryPopTarget()
        {
            var target = _targeting.Target;
            if (target == null) return SkillInputResult.InProgress;

            var executed = _soulOrbRoot.TryPop(target, _comboRoot.Current);
            return executed ? SkillInputResult.Executed : SkillInputResult.InProgress;
        }
    }
}