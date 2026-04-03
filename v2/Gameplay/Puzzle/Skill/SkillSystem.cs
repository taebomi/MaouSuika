using System;
using System.Collections;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    public class SkillSystem : MonoBehaviour
    {
        [SerializeField] private SkillBase skill;
        [SerializeField] private SkillGaugeConfigSO config;
        [SerializeField] private SkillGaugeView gaugeView;

        private readonly SkillModel _skillModel = new();
        private SkillExecutionContext _executionContext;
        private bool _isExecuting;

        public event Action SkillReady;

        private void OnEnable()
        {
            _skillModel.GaugeChanged += OnGaugeChanged;
            _skillModel.SkillActivated += OnSkillActivated;
        }

        private void OnDisable()
        {
            if (_skillModel != null)
            {
                _skillModel.GaugeChanged -= OnGaugeChanged;
                _skillModel.SkillActivated -= OnSkillActivated;
            }
        }

        public void Initialize(SkillExecutionContext context)
        {
            _executionContext = context;
            _skillModel.Initialize(skill.MaxGauge);
        }

        public void Setup()
        {
            _isExecuting = false;
            _skillModel.Setup();
            gaugeView.Setup(0f);
        }

        public void HandleComboTriggered(ComboEvent comboEvent)
        {
            if (_isExecuting) return;
            _skillModel.AddGauge(config.GetGaugeAmount(comboEvent.Combo));
        }

        public void TryActivate()
        {
            if (!_skillModel.IsSkillReady || _isExecuting) return;
            _isExecuting = true;
            StartCoroutine(ExecuteSkillRoutine());
        }

        private IEnumerator ExecuteSkillRoutine()
        {
            yield return skill.Execute(_executionContext);

            if (!skill.WasCancelled)
                _skillModel.ConsumeGauge();

            _isExecuting = false;
        }

        private void OnGaugeChanged(SkillGaugeEvent e)
        {
            gaugeView.UpdateGauge(e.NormalizedGauge);
        }

        private void OnSkillActivated()
        {
            SkillReady?.Invoke();
        }
    }
}
