using System;
using TBM.MaouSuika.Gameplay.Puzzle;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    public class ComboModel
    {
        public int CurrentCombo { get; private set; }

        private bool _shouldKeepCombo;

        public event Action<ComboFailedEvent> ComboFailed;
        public event Action<ComboEvent> ComboTriggered;

        public void Setup(int initialCombo = 0)
        {
            _shouldKeepCombo = true;
            CurrentCombo = initialCombo;
        }

        public void HandleSuikaMerged(MergeEvent mergeEvent)
        {
            _shouldKeepCombo = true;
            CurrentCombo++;

            if (ComboUtility.IsCombo(CurrentCombo))
            {
                var comboEvent = new ComboEvent(CurrentCombo, mergeEvent.Pos);
                ComboTriggered?.Invoke(comboEvent);
            }
        }

        public void HandleShooterFired()
        {
            if (_shouldKeepCombo)
            {
                _shouldKeepCombo = false;
                return;
            }

            var lastCombo = CurrentCombo;
            CurrentCombo = 0;

            if (!ComboUtility.IsCombo(lastCombo)) return;

            var comboFailedEvent = new ComboFailedEvent(lastCombo);
            ComboFailed?.Invoke(comboFailedEvent);
        }
    }
}