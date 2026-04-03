using System;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    [CreateAssetMenu(fileName = "DangerLevelEventChannelSO",
        menuName = "Maou Suika/Puzzle/Game Over/Danger Level Event Channel")]
    public class DangerLevelEventChannelSO : ScriptableObject
    {
        public event Action<int, DangerLevel> EventRaised;

        public void RaiseEvent(int playerIndex, DangerLevel level)
        {
            EventRaised?.Invoke(playerIndex, level);
        }
    }
}