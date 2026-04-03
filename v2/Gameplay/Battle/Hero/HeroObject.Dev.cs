#if UNITY_EDITOR
using Sirenix.OdinInspector;
using UnityEditor;

namespace TBM.MaouSuika.Gameplay.Battle
{
    public partial class HeroObject
    {
        [Button]
        private void AutoSerialize()
        {
            if (data == null) Dev_SerializeHeroData();
        }

        private void Dev_SerializeHeroData()
        {
            var guids = AssetDatabase.FindAssets($"{name} t:HeroDataSO");
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var so = AssetDatabase.LoadAssetAtPath<HeroDataSO>(path);
                if (so != null && so.name == name)
                {
                    data = so;
                    return;
                }
            }

            Logger.Warning($"[{name}] HeroDataSO not found.");
        }

        [Button]
        private void PlayAttack()
        {
            visualController.Play(HeroAnimType.Attack);
        }
    }
}
#endif