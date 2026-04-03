using System;
using SOSG.Stage.Player;
using UnityEngine;

namespace SOSG.Stage.GameOver
{
    [CreateAssetMenu(menuName = "SOSG/Game Play/GameOver/PlayerGameOverEventSO", fileName = "PlayerGameOverEventSO")]
    public class PlayerGameOverEventSO : ScriptableObject
    {
        public event Action<PlayerManager> GameOver;

        public void Invoke(PlayerManager playerManager)
        {
            GameOver?.Invoke(playerManager);
        }
    }
}