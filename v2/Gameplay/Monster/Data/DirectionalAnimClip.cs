using System;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Monster
{
    [Serializable]
    public struct DirectionalAnimClip
    {
        public AnimationClip left;
        public AnimationClip right;

        public DirectionalAnimClip(AnimationClip left, AnimationClip right)
        {
            this.right = right;
            this.left = left;
        }

        public AnimationClip GetClip(bool isFacingRight) => isFacingRight ? right : left;
    }
}