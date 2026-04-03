#if UNITY_EDITOR
using System;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Monster
{
    public partial class MonsterVisualController
    {
        [OnValueChanged(nameof(Dev_Play))]
        [BoxGroup("Dev")] [SerializeField]
        private bool dev_isFacingRight;
        [OnValueChanged(nameof(Dev_Play))]
        [BoxGroup("Dev")] [EnumToggleButtons] [SerializeField]
        private Dev_MonsterVisualControllerExtensions.Dev_AnimType dev_curAnimType;

        [BoxGroup("Dev")] [Button(Name = "Play")]
        private void Dev_Play()
        {
            PlayAnim(dev_curAnimType.Convert(), dev_isFacingRight);
        }

        [BoxGroup("Dev")] [Button(Name = "Stop")]
        private void Dev_Stop()
        {
            var state = PlayAnim(MonsterAnimType.Idle, true);
            state.NormalizedTime = 0f;
            state.IsPlaying = false;
        }
    }

    public static class Dev_MonsterVisualControllerExtensions
    {
        public enum Dev_AnimType
        {
            Idle,
            Move,
            Hit,
            Die,
        }

        public static MonsterAnimType Convert(this Dev_AnimType type)
        {
            return type switch
            {
                Dev_AnimType.Idle => MonsterAnimType.Idle,
                Dev_AnimType.Move => MonsterAnimType.Move,
                Dev_AnimType.Hit => MonsterAnimType.Hit,
                Dev_AnimType.Die => MonsterAnimType.Die,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }
    }
}

#endif