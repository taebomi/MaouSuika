using System.Collections;
using TBM.Core;
using TBM.Core.Coroutines;
using TBM.MaouSuika.Gameplay.Monster;
using TBM.MaouSuika.Gameplay.Unit;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Battle
{
    public partial class BattleMonster : MonoBehaviour, IDamagable
    {
        [SerializeField] private BattleConfigSO battleConfig;
        [SerializeField] private MonsterDataSO data;

        [SerializeField] private MonsterVisualController visualController;
        [SerializeField] private Rigidbody2D rb;

        private BattleSide _side;
        private Vector2 _moveDir;

        private int _curHp;

        private bool IsDead => _curHp <= 0;
        private bool IsKnockbacking => _hitCoroutine != null;

        private Coroutine _hitCoroutine;

        private void OnEnable()
        {
            visualController.DieAnimEnded += OnDieAnimEnded;
        }

        private void OnDisable()
        {
            visualController.DieAnimEnded -= OnDieAnimEnded;
        }

        public void Setup(int tier, BattleSide side)
        {
            _curHp = data.stats.maxHp;

            _side = side;
            _moveDir = _side.ToMoveDirection();
            gameObject.layer = _side.ToLayer();
            visualController.SetSortingOrder(-tier);
        }

        public void Move()
        {
            rb.linearVelocity = _moveDir * data.stats.moveSpeed;
            visualController.Play(MonsterAnimType.Move, _moveDir);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.TryGetComponent(out IDamagable target)) return;

            target.TakeDamage(data.stats.attackDamage);
        }


        public void TakeDamage(int damage)
        {
            if (IsDead) return;

            _curHp -= damage;
            if (IsDead)
            {
                OnDie();
                return;
            }

            OnHit();
        }

        private void OnHit()
        {
            if (IsKnockbacking) return;
            _hitCoroutine = StartCoroutine(KnockbackRoutine());
        }

        private IEnumerator KnockbackRoutine()
        {
            visualController.Play(MonsterAnimType.Hit);
            rb.linearVelocity = Vector2.zero;

            var knockbackDist = battleConfig.GetKnockbackDist(data.grade);
            var knockbackDir = -_moveDir;
            var startPos = rb.position;
            var endPos = startPos + knockbackDir * knockbackDist;

            var elapsed = 0f;
            var duration = battleConfig.KnockbackDuration;
            while (elapsed < duration)
            {
                elapsed += Time.fixedDeltaTime;
                var ease = Easing.OutQuad(elapsed / duration);
                rb.MovePosition(Vector2.Lerp(startPos, endPos, ease));
                yield return YieldCache.WaitForFixedUpdate;
            }

            Move();
            _hitCoroutine = null;
        }

        private void OnDie()
        {
            StopAllCoroutines();

            rb.simulated = false;
            visualController.Play(MonsterAnimType.Die);
        }

        private void OnDieAnimEnded()
        {
            Destroy(gameObject);
        }
    }
}