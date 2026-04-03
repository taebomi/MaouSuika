using System.Collections;
using System.Collections.Generic;
using SOSG.Stage;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GashaponComboSO))]
public class GashaponComboSOCustomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        var gashaponComboSO = (GashaponComboSO) target;
        GUILayout.Label($"Value - {gashaponComboSO.Value}");
        if (GUILayout.Button("Add"))
        {
            gashaponComboSO.Increase();
        }
        
        Repaint();
    }
}
