using System.Collections;
using TBM.MaouSuika.Gameplay.Monster;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    public class SuikaMonster : MonoBehaviour
    {
        [SerializeField] private SuikaAnimProfileSO animProfile;

        public MonsterVisualController VisualController { get; private set; }

        private Coroutine _hitRoutine;
        private float _nextHitAvailableTime;
        private bool IsHit => _hitRoutine != null;

        private MonsterAnimType _mainAnimType;
        private bool _isRightDirection;

        public void Setup(MonsterVisualController visualController, MonsterDataSO data, float shellSize)
        {
            VisualController = visualController;

            _isRightDirection = Random.value > 0.5f;
            FitToShell(data, shellSize, _isRightDirection);
            StartCoroutine(LoopAnimationRoutine());
        }

        public void OnCollided()
        {
            if (Time.time < _nextHitAvailableTime || IsHit) return;

            _hitRoutine = StartCoroutine(HitRoutine());
        }

        public void SetColor(Color color)
        {
            VisualController.SetColor(color);
        }

        public void SetSortingLayerID(int id)
        {
            VisualController.SetSortingId(id);
        }

        private void FitToShell(MonsterDataSO data, float shellDiameter, bool isRightDirection)
        {
            var scaleFactor = Mathf.Min(shellDiameter / data.size, 1f);
            var centerOffset = data.centerOffset * scaleFactor;
            if (!isRightDirection) centerOffset.x *= -1f;

            VisualController.SetParent(transform);
            VisualController.SetLocalPosition(centerOffset);
            VisualController.SetLocalScale(new Vector3(scaleFactor, scaleFactor, 1f));
        }

        private IEnumerator LoopAnimationRoutine()
        {
            while (true)
            {
                yield return StartCoroutine(IdleRoutine());
                yield return StartCoroutine(MoveRoutine());
            }
        }

        private IEnumerator IdleRoutine()
        {
            _mainAnimType = MonsterAnimType.Idle;
            VisualController.Play(MonsterAnimType.Idle, _isRightDirection);

            var duration = animProfile.GetRandomIdleTime();
            while (duration > 0f)
            {
                if (!IsHit) duration -= Time.deltaTime;
                yield return null;
            }
        }

        private IEnumerator MoveRoutine()
        {
            _mainAnimType = MonsterAnimType.Move;
            VisualController.Play(MonsterAnimType.Move, _isRightDirection);

            var duration = animProfile.GetRandomMoveTime();
            var rotationSpeed = animProfile.GetRandomMoveRotationSpeed();
            rotationSpeed = _isRightDirection ? -rotationSpeed : rotationSpeed;
            while (duration > 0f)
            {
                if (!IsHit) duration -= Time.deltaTime;
                
                transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
                yield return null;
            }
        }

        private IEnumerator HitRoutine()
        {
            _nextHitAvailableTime = Time.time + animProfile.HitCooldown;

            yield return VisualController.Play(MonsterAnimType.Hit);
            VisualController.Play(_mainAnimType, _isRightDirection);

            _hitRoutine = null;
        }
    }
}