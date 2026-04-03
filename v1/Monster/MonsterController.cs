using TaeBoMi.Pool;
using UnityEngine;

namespace SOSG.Monster
{
    public class MonsterController : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private Animator ani;
        [SerializeField] private SpriteRenderer bodySr;
        [SerializeField] private Transform bodyTr;
        [SerializeField] private Transform shadowTr;

        private Vector3 _footPos;

        public MonsterDataSO Data { get; private set; }
        public int CurLevel { get; private set; }

        private IObjectPool<MonsterController> _pool;

        public Vector3 GetFootPosition()
        {
            return transform.position + _footPos;
        }

        public void Initialize(IObjectPool<MonsterController> pool)
        {
            _pool = pool;
        }

        //임시 함수, 전투 시스테믕로 바꿀 시에 수정해줘야 함.
        public void Initialize(MonsterDataSO data)
        {
            Data = data;
            ani.runtimeAnimatorController = data.animatorOverrideController;
            _footPos = data.footPos;
            transform.position -= data.footPos;
        }


        // 임시 함수, 전투 시스템으로 바꿀 시에 수정해줘야 함.
        public void Summon()
        {
            gameObject.SetActive(true);
            ani.SetBool(AnimatorCache.IsMove, true);
        }

        public void SetIdle()
        {
            ani.SetBool(AnimatorCache.IsMove, false);
            rb.linearVelocity = Vector2.zero;
        }

        public void SetMove()
        {
            ani.SetBool(AnimatorCache.IsMove, true);
            rb.linearVelocity = new Vector2(Data.speed, 0f);
        }

        public void Jump()
        {
            if (ani.GetCurrentAnimatorStateInfo(0).shortNameHash != AnimatorCache.Jump)
            {
                ani.SetTrigger(AnimatorCache.JumpTrigger);
            }
        }

        public void RotateJump()
        {
            if (ani.GetCurrentAnimatorStateInfo(0).shortNameHash != AnimatorCache.RotateJump)
            {
                ani.SetTrigger(AnimatorCache.RotateJumpTrigger);
            }
        }

        public void SetAnimationLoop(bool value)
        {
            ani.SetBool(AnimatorCache.Loop, value);
        }

        public void Set(MonsterDataSO data)
        {
            Data = data;
            ani.runtimeAnimatorController = data.animatorOverrideController;
            _footPos = data.footPos;
        }

        public void Set(MonsterDataSO data, int level)
        {
            Data = data;
            gameObject.SetActive(true);
            _footPos = data.footPos;
            CurLevel = level;
            ani.runtimeAnimatorController = data.animatorOverrideController;
            ani.SetBool(AnimatorCache.IsMove, true);
            // rb.velocity = Vector2.right * data.speed;
        }

        public void SetColor(Color color)
        {
            bodySr.color = color;
        }

        public void SetPositionToFoot()
        {
            transform.position -= _footPos;
        }

        public void SetPosition(Vector3 pos)
        {
            transform.position = pos + _footPos;
        }

        public void SetLocalPosition(Vector3 pos)
        {
            transform.localPosition = pos - _footPos;
        }

        public static MonsterGrade GetGrade(int level)
        {
            return level switch
            {
                <= (int)MonsterGrade.Common => MonsterGrade.Common,
                <= (int)MonsterGrade.Uncommon => MonsterGrade.Uncommon,
                <= (int)MonsterGrade.Rare => MonsterGrade.Rare,
                _ => MonsterGrade.Epic
            };
        }
    }
}