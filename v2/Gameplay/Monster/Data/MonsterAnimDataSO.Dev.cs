#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Monster
{
    public partial class MonsterAnimDataSO
    {
        [Button("Auto Load Clips From Folder (Clear Existing)", ButtonSizes.Large), PropertySpace(30)]
        private void AutoLoadClipsAll()
        {
            idleClip = default;
            moveClip = default;
            hitClip = default;
            dieClip = null;
            AutoLoadClips();
        }

        [Button("Auto Load Clips From Folder", ButtonSizes.Large), PropertySpace(30)]
        private void AutoLoadClips()
        {
            var assetPath = AssetDatabase.GetAssetPath(this);
            var folderPath = Path.GetDirectoryName(assetPath);
            if (string.IsNullOrEmpty(folderPath)) return;

            Undo.RecordObject(this, "Auto Load Monster Anim Clips");

            var rightClips = new Dictionary<MonsterAnimType, AnimationClip>();
            var leftClips = new Dictionary<MonsterAnimType, AnimationClip>();
            var count = 0;

            var guids = AssetDatabase.FindAssets("t:AnimationClip", new[] { folderPath });
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
                if (clip == null) continue;
                if (ParseClip(clip, rightClips, leftClips)) count++;
            }

            idleClip = BuildDirectional(MonsterAnimType.Idle, rightClips, leftClips);
            moveClip = BuildDirectional(MonsterAnimType.Move, rightClips, leftClips);
            hitClip = BuildDirectional(MonsterAnimType.Hit, rightClips, leftClips);

            EditorUtility.SetDirty(this);
            Debug.Log($"<color=green>Auto Load Monster Anim Clips Success</color>\nAssigned {count} clips");
        }

        private bool ParseClip(AnimationClip clip,
            Dictionary<MonsterAnimType, AnimationClip> rightClips,
            Dictionary<MonsterAnimType, AnimationClip> leftClips)
        {
            var parts = clip.name.Split('_');
            if (parts.Length < 2) return false;

            var lastPart = parts[^1];
            string animTypeString;
            bool? isRight = null;

            if (lastPart.Equals("Right", StringComparison.OrdinalIgnoreCase))
            {
                isRight = true;
                animTypeString = parts[^2];
            }
            else if (lastPart.Equals("Left", StringComparison.OrdinalIgnoreCase))
            {
                isRight = false;
                animTypeString = parts[^2];
            }
            else
            {
                animTypeString = lastPart;
            }

            if (!Enum.TryParse(animTypeString, true, out MonsterAnimType animType))
            {
                Debug.LogWarning($"Failed to parse anim type from [{clip.name}]");
                return false;
            }

            if (animType == MonsterAnimType.Die)
            {
                dieClip = clip;
                return true;
            }

            if (isRight == true)       rightClips[animType] = clip;
            else if (isRight == false) leftClips[animType] = clip;
            else
            {
                rightClips[animType] = clip;
                leftClips[animType] = clip;
            }

            return true;
        }

        private static DirectionalAnimClip BuildDirectional(
            MonsterAnimType type,
            Dictionary<MonsterAnimType, AnimationClip> rightClips,
            Dictionary<MonsterAnimType, AnimationClip> leftClips)
        {
            rightClips.TryGetValue(type, out var right);
            leftClips.TryGetValue(type, out var left);
            return new DirectionalAnimClip(left, right);
        }
    }
}
#endif