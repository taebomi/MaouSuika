using System;
using System.IO;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace TBM.MaouSuika.Core.Scene
{
    [CreateAssetMenu(fileName = "SceneConfigSO", menuName = "TBM/Core/Scene/Scene Config")]
    public class SceneConfig : ScriptableObject
    {
        [field: SerializeField] public TransitionType transitionIn { get; private set; } = TransitionType.Fade;
        [field: SerializeField] public TransitionType transitionOut { get; private set; } = TransitionType.Fade;

#if UNITY_EDITOR
        
        [field: SerializeField, ReadOnly] public string editor_sceneName { get; private set; }
        
        private void OnValidate()
        {
            var path = AssetDatabase.GetAssetPath(this);
            var fileName = Path.GetFileNameWithoutExtension(path);

            if (editor_sceneName != fileName)
            {
                editor_sceneName = fileName;
                EditorUtility.SetDirty(this);
            }
        }
#endif
    }
}