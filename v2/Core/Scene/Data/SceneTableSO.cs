using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace TBM.MaouSuika.Core.Scene
{
    [CreateAssetMenu(fileName = "SceneTable", menuName = "TBM/Core/Scene/Scene Table")]
    public class SceneTableSO : ScriptableObject
    {
        [SerializeField] private SerializedDictionary<string, SceneConfig> configs;
        [SerializeField] private SceneConfig defaultConfig;

        public SceneConfig GetConfig(string sceneName)
        {
            return configs.GetValueOrDefault(sceneName, defaultConfig);
        }

#if UNITY_EDITOR

        [Button]
        private void Editor_AddSceneConfig(SceneConfig config)
        {
            if (!configs.TryAdd(config.editor_sceneName, config))
            {
                Debug.LogWarning($"Already Exists");
            }

            EditorUtility.SetDirty(this);
        }
#endif
    }
}