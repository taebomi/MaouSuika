using System;
using System.Collections;
using Animancer;
using TBM.MaouSuika.Gameplay.Unit;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Battle
{
    public partial class HeroObject : MonoBehaviour
    {
        [SerializeField] private HeroDataSO data;

        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private HeroVisualController visualController;
        [SerializeField] private HeroAttackBox attackBox;

        private int _currentHp;

        private float _lastAttackTime;
        private Coroutine _attackCoroutine;

        public event Action<HeroObject> OnDied;

        private bool IsDead => _currentHp <= 0;

        private void OnEnable()
        {
            visualController.AttackHit += OnAttacked;
        }

        private void OnDisable()
        {
            visualController.AttackHit -= OnAttacked;
        }

        public void Setup()
        {
            _currentHp = data.Stats.maxHp;
            visualController.Play(HeroAnimType.Idle);
        }

        private void Update()
        {
            CheckAttackRange();
        }

        private void CheckAttackRange()
        {
            if (!attackBox.HasTargets) return;

            var canAttack = _attackCoroutine == null && Time.time >= _lastAttackTime + data.Stats.attackCooldown;
            if (!canAttack) return;

            _attackCoroutine = StartCoroutine(AttackRoutine());
        }

        private IEnumerator AttackRoutine()
        {
            _lastAttackTime = Time.time;
            yield return visualController.Play(HeroAnimType.Attack);
            visualController.Play(HeroAnimType.Idle);
            _attackCoroutine = null;
        }

        private void OnAttacked()
        {
            OptionalWarning.EventPlayMismatch.Disable(); // animancer warning 방지

            foreach (var target in attackBox.Targets)
            {
                target.TakeDamage(data.Stats.attackDamage);
            }

            OptionalWarning.EventPlayMismatch.Enable();
        }

        public void OnContacted(IDamagable target)
        {
            target.TakeDamage(data.Stats.attackDamage);
        }


        public void TakeDamage(int damage)
        {
            if (IsDead) return;

            _currentHp -= damage;
            if (!IsDead)
            {
                OnDamaged();
                return;
            }

            OnDie();
        }

        private void OnDamaged()
        {
            visualController.FlashRed();

            if (visualController.CurAnimType != HeroAnimType.Idle) return;
            visualController.Play(HeroAnimType.Hit);
        }

        private void OnDie()
        {
            StopAllCoroutines();

            rb.simulated = false;
            _currentHp = 0;

            StartCoroutine(DieRoutine());
        }

        private IEnumerator DieRoutine()
        {
            yield return visualController.PlayDie();
            OnDied?.Invoke(this);
            Destroy(gameObject);
        }
    }
}