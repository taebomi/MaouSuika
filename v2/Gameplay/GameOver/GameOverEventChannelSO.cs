using System;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    [CreateAssetMenu(fileName = "GameOverEventChannelSO", menuName = "Maou Suika/Puzzle/Game Over/Game Over Event Channel")]
    public class GameOverEventChannelSO : ScriptableObject
    {
        public event Action<int> EventRaised;

        public void RaiseEvent(int playerIndex) => EventRaised?.Invoke(playerIndex);
    }
}