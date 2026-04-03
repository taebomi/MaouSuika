using System;
using UnityEngine;

namespace SOSG.Stage.GameOver
{
    [CreateAssetMenu(fileName = "GameOverSystemStateSO", menuName = "TaeBoMi/Stage/Game Over System State")]
    public class GameOverSystemStateSO : ScriptableObject
    {
        public event Action<GameOverSystemState> ActionOnStateChanged;
        public event Action ActionOnGameOver;
        
        public GameOverSystemState CurState { get; private set; }
        public GameOverSystemState PrevState { get; private set; }

#if UNITY_EDITOR
        [TextArea, SerializeField] private string memo;
        [SerializeField] private bool debugMode;
#endif

        public void Initialize()
        {
            PrevState = CurState = GameOverSystemState.Safe;
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public void ChangeState(GameOverSystemState gameOverSystemState)
        {
#if UNITY_EDITOR
            if (CurState == gameOverSystemState)
            {
                Debug.LogWarning($"{GetType()} - 이미 {gameOverSystemState} 상태입니다.");
                return;
            }
            if (debugMode)
            {
                Debug.Log($"{GetType()} - {gameOverSystemState}로 변경되었음.");
            }
#endif
            if(CurState == gameOverSystemState)
            {
                return;
            }
            
            PrevState = CurState;
            CurState = gameOverSystemState;
            ActionOnStateChanged?.Invoke(gameOverSystemState);
            if (gameOverSystemState is GameOverSystemState.GameOver)
            {
                ActionOnGameOver?.Invoke();
            }
        }
    }

    public enum GameOverSystemState
    {
        Safe,
        Warning,
        Countdown,
        GameOver,
    }
}