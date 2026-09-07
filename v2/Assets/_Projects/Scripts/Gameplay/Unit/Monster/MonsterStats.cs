using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MaouSuika.Gameplay
{
    [Serializable]
    public struct MonsterStats
    {
        private const float ATTACK_START_RANGE_MIN_RATIO = 0.55f;
        private const float ATTACK_START_RANGE_MAX_RATIO = 0.85f;
        private const float ATTACK_START_RANGE_JITTER = 0.25f;
        public int maxHp;
        public int attackDamage;
        public float moveSpeed;

        public float attackRange;
        public float attackInterval;

        public MonsterAttackType attackType;

        [Tooltip("Ram: 공격 후 자기 반동 거리")]
        public float recoilDistance;

        [Tooltip("공격 쿨다운 동안 이동 여부")]
        public bool moveDuringCooldown;

        public float RollAttackStartRange()
        {
            var center = attackRange * (ATTACK_START_RANGE_MAX_RATIO + ATTACK_START_RANGE_MIN_RATIO) * 0.5f;
            var limit = attackRange * (ATTACK_START_RANGE_MAX_RATIO - ATTACK_START_RANGE_MIN_RATIO) * 0.5f;
            var jitter = Mathf.Min(ATTACK_START_RANGE_JITTER, limit);
            return center + Random.Range(-jitter, jitter);
        }
    }
}