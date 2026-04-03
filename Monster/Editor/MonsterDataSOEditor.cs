using SOSG.Monster;
using UnityEditor;

[CustomEditor(typeof(MonsterDataSO))]
public class MonsterDataSOEditor : Editor
{
    private SerializedProperty _monsterID;

    private void OnEnable()
    {
        _monsterID = serializedObject.FindProperty("id");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        _monsterID.stringValue = serializedObject.targetObject.name;
        serializedObject.ApplyModifiedProperties();
    }
}
