using SOSG.Stage.GameOver;
using UnityEngine;

public class StageDebugManager : MonoBehaviour
{
    [SerializeField] private GameOverSystemStateSO gameOverSystemStateSO;
    
    public void OnGameOverBtnClicked()
    {
        gameOverSystemStateSO.ChangeState(GameOverSystemState.GameOver);
    }
}
