#if UNITY_EDITOR

using Sirenix.OdinInspector;
using TBM.MaouSuika.Gameplay.Battle;
using UnityEditor;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Monster
{
    public partial class MonsterDataSO
    {
        [BoxGroup("Dev")] [Button]
        private void AutoSerialize()
        {
            id = name;

            TryAssignPrefab(name, ref battlePrefab);
            TryAssignPrefab($"{name}_Visual", ref visualPrefab);
        }

        private void TryAssignPrefab<T>(string prefabName, ref T field) where T : Component
        {
            if (field != null) return;

            var guids = AssetDatabase.FindAssets($"{prefabName} t:Prefab");
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var prefab = AssetDatabase.LoadAssetAtPath<T>(path);
                if (prefab != null && prefab.name == prefabName)
                {
                    field = prefab;
                    return;
                }
            }

            Debug.LogWarning($"[{name}] Prefab '{prefabName}' not found.", this);
        }
    }
}

#endif