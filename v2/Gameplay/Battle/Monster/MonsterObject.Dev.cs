#if UNITY_EDITOR
using Sirenix.OdinInspector;
using TBM.MaouSuika.Gameplay.Monster;
using UnityEditor;

namespace TBM.MaouSuika.Gameplay.Battle
{
    public partial class BattleMonster
    {
        [Button]
        private void AutoSerialize()
        {
            if (data == null) Dev_SerializeMonsterData();

            if (visualController == null) visualController = GetComponentInChildren<MonsterVisualController>();
        }

        private void Dev_SerializeMonsterData()
        {
            var guids = AssetDatabase.FindAssets($"{name} t:MonsterDataSO");
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var so = AssetDatabase.LoadAssetAtPath<MonsterDataSO>(path);
                if (so != null && so.name == name)
                {
                    data = so;
                    return;
                }
            }

            Logger.Warning($"[{name}] MonsterDataSO not found.");
        }
    }
}

#endif