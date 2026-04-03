using System;
using UnityEngine;

namespace SOSG.Stage
{
[CreateAssetMenu(menuName = "TaeBoMi/Stage/Stage Manager SO", fileName = "StageManagerSO")]
    public class StageManagerSO : ScriptableObject
    {
        public Action ActionOnStageStarted;
        public Action ActionOnStageEnded;

        public State state;

        public enum State
        {
            Playing,
            GameOver,
        }

        public void StartStage()
        {
            state = State.Playing;
            ActionOnStageStarted?.Invoke();
        }
        
        public void EndStage()
        {
            state = State.GameOver;
            ActionOnStageEnded?.Invoke();
        }
    }
}