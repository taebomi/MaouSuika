using FMODUnity;
using TBM.MaouSuika.Core;
using TBM.MaouSuika.Core.Audio;
using TBM.MaouSuika.Core.Vibration;
using TBM.MaouSuika.Data;
using TBM.MaouSuika.Gameplay.Monster;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    public class SuikaObject : MonoBehaviour
    {
        [SerializeField] private EventReference collidedSfx;

        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private CircleCollider2D coll;

        [SerializeField] private SuikaShell shell;
        [SerializeField] private SuikaMonster monster;

        public MonsterVisualController MonsterVisual => monster.VisualController;

        public SuikaTierData Data { get; private set; }
        public int CreationOrder { get; private set; }
        public bool IsGrounded { get; private set; }

        public MonsterDataSO MonsterData => Data.MonsterData;
        public int Tier => Data.Tier;

        public Vector2 RbPos => rb.position;
        public float Size => Data.Size;
        public float Radius => Size * 0.5f;

        private MergeSystem _mergeSystem;


        public void Initialize(MergeSystem mergeSystem)
        {
            _mergeSystem = mergeSystem;
        }

        public void Setup(SuikaObjectSetupData setupData, MonsterVisualController monsterVisual)
        {
            IsGrounded = false;

            Data = setupData.TierData;
            CreationOrder = setupData.CreationOrder;

            coll.radius = Radius;
            rb.mass = Data.Mass;

            shell.Setup(Size);
            monster.Setup(monsterVisual, MonsterData, Size);
            SetColor(MonsterData.suikaColor, Color.white);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            PlayCollidedEffect();

            if (other.gameObject.TryGetComponent(out SuikaObject otherSuika))
            {
                HandleSuikaCollision(otherSuika);
            }
            else if (other.gameObject.CompareTag(Tags.GROUND))
            {
                IsGrounded = true;
            }
        }

        public void SetParent(Transform parent, bool worldPositionStays = false)
        {
            transform.SetParent(parent, worldPositionStays);
        }

        /// <summary>
        /// 물리 연산 꺼진 상태에서 transform.position으로 조작
        /// </summary>
        /// <param name="position"></param>
        public void SetVisualPosition(Vector3 position)
        {
            transform.position = position;
        }

        public void SetGrounded(bool value)
        {
            IsGrounded = value;
        }

        public void Rotate(float angle)
        {
            transform.Rotate(0f, 0f, angle);
        }

        public void Shoot(Vector2 velocity)
        {
            SetState_Shot();

            rb.linearVelocity = velocity;
            rb.angularVelocity = velocity.x;
        }

        public void SetColor(Color core, Color detail)
        {
            shell.SetCoreColor(core);
            shell.SetDetailColor(detail);
            monster.SetColor(detail);
        }

        #region State

        public void SetState_Loaded()
        {
            ResetVisual();
            SetSortingLayerID(SortingLayers.OBJECT_FRONT);

            ResetPhysics();
            rb.simulated = false;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        #region Loaded State 전용

        public void UpdateLoadedVisual(bool isBlocked, bool isCooldown)
        {
            var coreColor = isBlocked ? Color.red : MonsterData.suikaColor;
            var detailColor = isBlocked ? Color.red : Color.white;
            var alpha = isCooldown ? 0.5f : 1f;

            coreColor.a = alpha;
            detailColor.a = alpha;

            SetColor(coreColor, detailColor);
        }

        #endregion

        private void SetState_Shot()
        {
            ResetVisual();
            SetSortingLayerID(SortingLayers.OBJECT);

            ResetPhysics();
            rb.simulated = true;
            rb.bodyType = RigidbodyType2D.Dynamic;
        }

        public void SetState_Merged()
        {
            ResetVisual();
            SetSortingLayerID(SortingLayers.OBJECT);

            ResetPhysics();
            rb.simulated = true;
            rb.bodyType = RigidbodyType2D.Dynamic;
        }

        public void SetState_MergeSource()
        {
            SetSortingLayerID(SortingLayers.OBJECT_FRONT);

            ResetPhysics();
            rb.simulated = false;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        public void SetState_MergeTarget()
        {
            SetSortingLayerID(SortingLayers.OBJECT);

            ResetPhysics();
            rb.simulated = true;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        #endregion

        // ################################################
        // PRIVATE
        // ################################################
        private void SetSortingLayerID(int id)
        {
            shell.SetSortingLayerID(id);
            monster.SetSortingLayerID(id);
        }

        private void ResetVisual()
        {
            SetColor(MonsterData.suikaColor, Color.white);
        }

        private void ResetPhysics()
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        private void PlayCollidedEffect()
        {
            monster.OnCollided();
            AudioManager.Instance.PlaySfx(collidedSfx);
            VibrationManager.Instance.Play(Data.CollisionHaptic);
        }

        private void HandleSuikaCollision(SuikaObject other)
        {
            if (other.IsGrounded) IsGrounded = true;

            _mergeSystem.RequestMerge(this, other);
        }
    }
}