using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AYellowpaper.SerializedCollections;
using Sirenix.OdinInspector;
using TBM.Core;
using UnityEditor;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Monster
{
    [CreateAssetMenu(fileName = "MonsterAnimData", menuName = "TBM/Monster/Anim Data")]
    public partial class MonsterAnimDataSO : ScriptableObject
    {
        [SerializeField] private DirectionalAnimClip idleClip;
        [SerializeField] private DirectionalAnimClip moveClip;
        [SerializeField] private DirectionalAnimClip hitClip;
        [SerializeField] private AnimationClip dieClip;

        public AnimationClip GetClip(MonsterAnimType type, bool isRight = true)
        {
            return type switch
            {
                MonsterAnimType.Idle => idleClip.GetClip(isRight),
                MonsterAnimType.Move => moveClip.GetClip(isRight),
                MonsterAnimType.Hit => hitClip.GetClip(isRight),
                MonsterAnimType.Die => dieClip,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }
    }
}