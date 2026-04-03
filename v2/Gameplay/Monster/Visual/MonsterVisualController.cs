using System;
using Animancer;
using DG.Tweening;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Monster
{
    public partial class MonsterVisualController : MonoBehaviour
    {
        [SerializeField] private MonsterAnimDataSO animData;
        [SerializeField] private MonsterVisualConfigSO visualConfig;

        [SerializeField] private AnimancerComponent animancer;
        [SerializeField] private SpriteRenderer sr;
        [SerializeField] private GameObject shadow;

        private AnimancerState _curState;
        private MonsterAnimType _curAnimType;
        private bool _isFacingRight;

        private Tween _victoryTween;
        private float _victoryOriginY;

        public event Action DieAnimEnded;

        private void Awake()
        {
            RegisterDieEvent();
        }

        public void SetParent(Transform parent, bool worldPositionStays = false)
        {
            transform.rotation = Quaternion.identity;
            transform.SetParent(parent, worldPositionStays);
        }

        public void SetLocalPosition(Vector3 pos) => transform.localPosition = pos;
        public void SetLocalScale(Vector3 scale) => transform.localScale = scale;
        public void SetColor(Color color) => sr.color = color;
        public void SetShadowActive(bool value) => shadow.SetActive(value);
        public void SetSortingId(int id) => sr.sortingLayerID = id;
        public void SetSortingOrder(int order) => sr.sortingOrder = order;

        public AnimancerState Play(MonsterAnimType animType, bool isRight)
        {
            if(_curAnimType != animType) return PlayAnim(animType, isRight);
            
            if (_isFacingRight == isRight) return _curState;

            var normalizedTime = _curState.NormalizedTime;
            PlayAnim(animType, isRight);
            _curState.NormalizedTime = normalizedTime;
            return _curState;

        }

        public AnimancerState Play(MonsterAnimType animType, Vector2 direction)
        {
            return Play(animType, direction.x >= 0f);
        }

        public AnimancerState Play(MonsterAnimType animType)
        {
            return Play(animType, _isFacingRight);
        }

        private AnimancerState PlayAnim(MonsterAnimType animType, bool isRight)
        {
            _curAnimType = animType;
            _isFacingRight = isRight;

            var clip = animData.GetClip(animType, isRight);
            _curState = animancer.Play(clip);

            return _curState;
        }

        public void PlayVictory(MonsterVictoryType type)
        {
            _victoryTween?.Kill();
            _victoryOriginY = transform.localPosition.y;

            if (type == MonsterVictoryType.SpinJump)
                Play(MonsterAnimType.Victory);

            _victoryTween = transform
                .DOLocalMoveY(visualConfig.VictoryJumpHeight, visualConfig.VictoryJumpDuration)
                .SetRelative().SetLoops(-1, LoopType.Yoyo).SetEase(Ease.OutQuad);
        }

        public void StopVictory()
        {
            _victoryTween?.Kill();
            _victoryTween = null;
            transform.DOLocalMoveY(_victoryOriginY, visualConfig.VictoryJumpDuration * 0.5f).SetEase(Ease.InQuad);
            Play(MonsterAnimType.Idle);
        }

        private void OnDestroy()
        {
            _victoryTween?.Kill();
        }

        private void RegisterDieEvent()
        {
            var dieClip = animData.GetClip(MonsterAnimType.Die);
            var dieState = animancer.States.GetOrCreate(dieClip);
            dieState.Events(this).OnEnd = () => DieAnimEnded?.Invoke();
        }
    }
}