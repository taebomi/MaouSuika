using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

#if UNITY_EDITOR
namespace SOSG.UI
{
    [CustomEditor(typeof(PressButtonWithText))]
    public class PressButtonWithTextEditor : ButtonEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("tmp"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("normalSprite"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("disabledSprite"));
            serializedObject.ApplyModifiedProperties();
        }
    }
}


#endif