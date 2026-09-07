using System;
using MaouSuika.Core;
using UnityEngine;

namespace MaouSuika.Gameplay
{
    [CreateAssetMenu(fileName = "MonsterSpriteAnimationSetSO",
        menuName = MenuPath.Gameplay.Unit.MONSTER + "Sprite Animation Set")]
    public partial class MonsterSpriteAnimationSetSO : ScriptableObject
    {
        [Header("Animations")]
        [SerializeField] private UnitDirectionalSpriteAnimation idle;
        [SerializeField] private UnitDirectionalSpriteAnimation move;
        [SerializeField] private UnitDirectionalSpriteAnimation attack;
        [SerializeField] private UnitDirectionalSpriteAnimation hit;
        [SerializeField] private UnitDirectionalSpriteAnimation die;

        [Header("Move")]
        [Tooltip("Move 한 사이클의 누적 이동 비율")]
        [SerializeField] private AnimationCurve moveGait;

        [Header("Attack")]
        [Range(0f, 1f)] [SerializeField] private float attackHitTime = .5f;

        public AnimationCurve MoveGait => moveGait;
        public float AttackHitTime => attackHitTime;

        public UnitSpriteAnimation Resolve(MonsterAnimationState state, UnitFacing facing)
        {
            return GetDirectionalClip(state).Resolve(facing);
        }

        public bool Has(MonsterAnimationState state)
        {
            return GetDirectionalClip(state).HasAnyFrame;
        }

        private UnitDirectionalSpriteAnimation GetDirectionalClip(MonsterAnimationState state)
        {
            return state switch
            {
                MonsterAnimationState.Idle => idle,
                MonsterAnimationState.Move => move,
                MonsterAnimationState.Attack => attack,
                MonsterAnimationState.Hit => hit,
                MonsterAnimationState.Die => die,
                _ => throw new ArgumentOutOfRangeException(nameof(state), state, null),
            };
        }
    }
}