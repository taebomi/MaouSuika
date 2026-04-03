using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace SOSG.Skill
{
    [CustomEditor(typeof(SkillSystemSO))]
    public class SkillSystemSOEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var skillSystemSO = target as SkillSystemSO;

            // gui text field skill gauge
            // when click button, skill gauge text will parameter
            if (GUILayout.Button("Charge Skill Gauge"))
            {
                skillSystemSO.RequestChargeSkillGauge();
            }

            serializedObject.ApplyModifiedProperties();
            Repaint();
        }
    }
}
#endif