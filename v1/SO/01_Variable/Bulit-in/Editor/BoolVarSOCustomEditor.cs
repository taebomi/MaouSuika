using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BoolVariableSO))]
public class BoolVarSOCustomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        var boolVarSO = (BoolVariableSO)target;
        GUILayout.Label($"Value - {boolVarSO.Value}");

        if (GUILayout.Button("Set True"))
        {
            boolVarSO.Set(true);
        }
        if(GUILayout.Button("Set False"))
        {
            boolVarSO.Set(false);
        }
    }
}
