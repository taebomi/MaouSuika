using System;
using UnityEngine;

namespace MaouSuika.Gameplay
{
    public class SkillRoot : MonoBehaviour
    {
        [SerializeField] private LayerMask soulOrbTargetLayerMask;
        [SerializeField] private SingleSoulOrbTargetingView singleSoulOrbTargetingView;

        [SerializeField] private SkillChargeSettingsSO chargeSettings;
        [SerializeField] private SkillView view;

        private SoulOrbRoot _soulOrbRoot;
        private ComboRoot _comboRoot;
        private ShooterRoot _shooterRoot;

        private ISkill _skill;
        private ISkillInputHandler _activeInputHandler;

        private bool _useRequested;
        private int _currentCharge;
        private SkillPhase _phase;

        public int CurrentCharge => _currentCharge;
        public int MaxCharge => _skill.Definition.RequiredCharge;
        public bool IsReady => _currentCharge >= MaxCharge;
        public bool IsAwaitingInput => _phase is SkillPhase.AwaitingInput;

        public void Initialize(
            SoulOrbRoot soulOrbRoot, ComboRoot comboRoot, ShooterRoot shooterRoot, SkillLoadout loadout)
        {
            _soulOrbRoot = soulOrbRoot;
            _comboRoot = comboRoot;
            _shooterRoot = shooterRoot;

            _skill = CreateSkill(loadout.EquippedSkill);

            view.Initialize(RequestUse);
        }

        public void Setup()
        {
            _useRequested = false;
            _currentCharge = 0;

            _activeInputHandler = null;
            _phase = SkillPhase.Idle;

            RefreshView();
        }

        public void Stop()
        {
            CancelPendingUse();
            view.SetInteractable(false);
        }

        public void Pause()
        {
            CancelPendingUse();
            view.SetInteractable(false);
        }

        public void Resume()
        {
            RefreshView();
        }

        private void CancelPendingUse()
        {
            _useRequested = false;
            if (_activeInputHandler != null)
            {
                _activeInputHandler.EndInput();
                _activeInputHandler = null;
            }

            _phase = SkillPhase.Idle;
        }

        public SkillUseResult ProcessUseRequest()
        {
            if (!_useRequested) return SkillUseResult.None;
            _useRequested = false;

            if (!IsReady) return SkillUseResult.None;

            var result = _skill.TryUse();

            switch (result)
            {
                case SkillUseResult.None:
                    return SkillUseResult.None;
                case SkillUseResult.Executed:
                    ConsumeCharge();
                    RefreshView();
                    return SkillUseResult.Executed;
                case SkillUseResult.AwaitingInput:
                    BeginInput();
                    return SkillUseResult.AwaitingInput;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void HandleInput(in SkillInputSnapshot input)
        {
            if (!IsAwaitingInput) return;

            var result = _activeInputHandler.HandleInput(input);

            switch (result)
            {
                case SkillInputResult.InProgress:
                    break;
                case SkillInputResult.Executed:
                    ConsumeCharge();
                    EndSkillInput();
                    break;
                case SkillInputResult.Canceled:
                    EndSkillInput();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void RequestUse()
        {
            if (_phase is not SkillPhase.Idle) return;

            _useRequested = true;
        }

        private void BeginInput()
        {
            if (_skill is not ISkillInputHandler inputHandler)
            {
                throw new InvalidOperationException();
            }

            _activeInputHandler = inputHandler;
            _phase = SkillPhase.AwaitingInput;
            RefreshView();
        }

        private void EndSkillInput()
        {
            _activeInputHandler.EndInput();
            _activeInputHandler = null;

            _phase = SkillPhase.Idle;

            RefreshView();
        }

        private void ConsumeCharge()
        {
            _currentCharge = 0;
        }

        public void AddChargeForCombo(int combo)
        {
            var gain = chargeSettings.GainFor(combo);
            _currentCharge = Mathf.Min(_currentCharge + gain, MaxCharge);
            RefreshView();
        }

        private void RefreshView()
        {
            var normalizedCharge = _currentCharge / (float)MaxCharge;

            view.SetCharge(normalizedCharge);
            view.SetInteractable(IsReady && _phase is SkillPhase.Idle);
        }


        // 나중에 팩토리로 추출
        private ISkill CreateSkill(SkillDefinitionSO definition)
        {
            return definition.Id switch
            {
                SkillIds.DISCARD_LOADED_SOUL_ORB => CreateDiscardLoadedSoulOrb(definition),
                SkillIds.POP_SOUL_ORB => CreatePopSoulOrb(definition),
                _ => throw new ArgumentOutOfRangeException($"Unsupported skill ID: {definition.Id}"),
            };
        }

        private ISkill CreateDiscardLoadedSoulOrb(SkillDefinitionSO definition)
        {
            return new DiscardLoadedSoulOrbSkill(definition, _soulOrbRoot, _shooterRoot);
        }

        private ISkill CreatePopSoulOrb(SkillDefinitionSO definition)
        {
            var navigator = new SoulOrbTargetNavigator(soulOrbTargetLayerMask, _soulOrbRoot.Spawner);
            var targeting = new SingleSoulOrbTargeting(navigator, singleSoulOrbTargetingView);
            return new PopSoulOrbSkill(definition, targeting, _soulOrbRoot, _comboRoot);
        }
    }
}