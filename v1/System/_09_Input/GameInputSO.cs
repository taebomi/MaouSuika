using System;
using UnityEngine;

namespace SOSG.System.Input
{
    [CreateAssetMenu(menuName = "TaeBoMi/Input/Game Input", fileName = "GameInputSO", order = 10000)]
    public class GameInputSO : ScriptableObject
    {
        public event Action<bool> ActionOnAllInputDisableRequested;
        public event Action ActionOnStageControlEnabled;
        public event Action ActionOnStageControlDisabled;
        private int _stageControlCount;

        public void InitializeStageControl()
        {
            _stageControlCount = 0;
            ActionOnStageControlEnabled?.Invoke();
        }

        public void DisableAllInputs(bool value)
        {
            ActionOnAllInputDisableRequested?.Invoke(value);
        }

        public void EnableStageControl()
        {
            _stageControlCount--;
            if (_stageControlCount == 0)
            {
                ActionOnStageControlEnabled?.Invoke();
            }
        }

        public void DisableStageControl()
        {
            _stageControlCount++;
            if (_stageControlCount == 1)
            {
                ActionOnStageControlDisabled?.Invoke();
            }
        }
    }
}