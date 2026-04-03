using System;
using TBM.MaouSuika.Gameplay.Puzzle;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    public class ComboSystem : MonoBehaviour
    {
        [SerializeField] private ComboFeedbackController feedbackController;

        public int CurCombo => _comboModel.CurrentCombo;

        private readonly ComboModel _comboModel = new();

        public event Action<ComboFailedEvent> ComboFailed;
        public event Action<ComboEvent> ComboTriggered;

        private void OnEnable()
        {
            _comboModel.ComboFailed += OnComboFailed;
            _comboModel.ComboTriggered += OnComboTriggered;
        }

        public void Setup(int initialCombo = 0)
        {
            _comboModel.Setup(initialCombo);
        }

        private void OnDisable()
        {
            if (_comboModel != null)
            {
                _comboModel.ComboFailed -= OnComboFailed;
                _comboModel.ComboTriggered -= OnComboTriggered;
            }
        }


        public void HandleSuikaMerged(MergeEvent mergeEvent)
        {
            _comboModel.HandleSuikaMerged(mergeEvent);
        }

        public void HandleShooterFired()
        {
            _comboModel.HandleShooterFired();
        }

        private void OnComboFailed(ComboFailedEvent comboFailedEvent)
        {
            // feedbackController.HandleComboFailed(comboFailedEvent);
            ComboFailed?.Invoke(comboFailedEvent);
        }

        private void OnComboTriggered(ComboEvent comboEvent)
        {
            // feedbackController.HandleComboTriggered(comboEvent);
            ComboTriggered?.Invoke(comboEvent);
        }
    }
}