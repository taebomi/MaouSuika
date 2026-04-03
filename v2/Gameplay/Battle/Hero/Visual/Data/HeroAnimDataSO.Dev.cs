#if UNITY_EDITOR
using System;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Battle
{
    public partial class HeroAnimDataSO
    {
        [Button("Auto Load Clips From Folder (Clear Existing)")]
        private void AutoLoadClipsAll()
        {
            idle = null;
            move = null;
            attack = null;
            AutoLoadClips();
        }

        [Button("Auto Load Clips From Folder")]
        private void AutoLoadClips()
        {
            var assetPath = AssetDatabase.GetAssetPath(this);
            var folderPath = System.IO.Path.GetDirectoryName(assetPath);
            if (string.IsNullOrEmpty(folderPath)) return;

            Undo.RecordObject(this, "Auto Load Hero Anim Clips");

            var guids = AssetDatabase.FindAssets("t:AnimationClip", new[] { folderPath });
            var count = 0;
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
                if (clip == null) continue;

                if (TryAssignClip(clip)) count++;
            }

            EditorUtility.SetDirty(this);
            Debug.Log($"<color=green>Auto Load Hero Anim Clips Success</color>\nAssigned {count} clips");
        }

        private bool TryAssignClip(AnimationClip clip)
        {
            var parts = clip.name.Split('_');
            foreach (var part in parts)
            {
                if (!Enum.TryParse(part, true, out HeroAnimType animType)) continue;

                switch (animType)
                {
                    case HeroAnimType.Idle:
                        idle = clip;
                        return true;
                    case HeroAnimType.Move:
                        move = clip;
                        return true;
                    case HeroAnimType.Attack:
                        attack = clip;
                        return true;
                    case HeroAnimType.Hit:
                        hit = clip;
                        return true;
                    case HeroAnimType.None:
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return false;
        }
    }
}
#endif