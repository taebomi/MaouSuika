using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

#if UNITY_EDITOR

namespace SOSG.UI
{
    [CustomEditor(typeof(PressButtonWithImage))]
    public class PressButtonWithImageEditor : ButtonEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("<ElementImage>k__BackingField"));
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}

#endif
