using SOSG.Stage.GameOver;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CustomEditor(typeof(GameOverSystemStateSO))]
public class GameOverSystemStateSOCustomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        var stateSO = (GameOverSystemStateSO)target;
        
        GUILayout.Label($"현재 상태 - {stateSO.CurState}");
        
        if(GUILayout.Button("Set State - Safe"))
        {
            stateSO.ChangeState(GameOverSystemState.Safe);
        }
        if(GUILayout.Button("Set State - Warning"))
        {
            stateSO.ChangeState(GameOverSystemState.Warning);
        }
        if(GUILayout.Button("Set State - Countdown"))
        {
            stateSO.ChangeState(GameOverSystemState.Countdown);
        }

        if(GUILayout.Button("Set State - GameOver"))
        {
            stateSO.ChangeState(GameOverSystemState.GameOver);
        }
        
        Repaint();
    }
}
#endif