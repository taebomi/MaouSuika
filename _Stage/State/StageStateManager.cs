using System;
using Cysharp.Threading.Tasks;
using SOSG.Stage.GameOver;
using SOSG.Stage.State;
using UnityEngine;
using UnityEngine.Serialization;

namespace SOSG.Stage.State
{
    public class StageStateManager : MonoBehaviour
    {
        [SerializeField] private PlayerGameOverEventSO playerGameOverEvent;

        public State CurState { get; private set; }

        public enum State
        {
            Init,
            Ready,
            Play,
            Pause,
            GameOver,
        }

        private void Awake()
        {
            
        }

        private void OnEnable()
        {
            playerGameOverEvent.GameOver += OnPlayerGameOver;
        }

        private void OnDisable()
        {
            playerGameOverEvent.GameOver -= OnPlayerGameOver;
        }
        
        
        
        
        
        
        
        

        private void OnPlayerGameOver(IPlayerGameOverHandler player)
        {
            ChangeState(State.GameOver);
        }


        private void ChangeState(State state)
        {
            if (CurState == state)
            {
                return;
            }
            
            
            CurState = state;
        }
    }
}