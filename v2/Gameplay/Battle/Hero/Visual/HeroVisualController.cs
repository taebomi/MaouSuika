using System;
using Animancer;
using DG.Tweening;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Battle
{
    public class HeroVisualController : MonoBehaviour
    {
        [SerializeField] private HeroVisualConfigSO visualConfig;
        [SerializeField] private HeroAnimDataSO animData;

        [SerializeField] private AnimancerComponent animancer;
        [SerializeField] private Transform bodyTr;
        [SerializeField] private Transform shadowTr;
        [SerializeField] private SpriteRenderer bodySr;

        public HeroAnimType CurAnimType { get; private set; }
        private AnimancerState _curState;

        private Tween _damageFlashTween;
        private Tween _victoryTween;
        private float _victoryOriginY;

        /// <summary>Attack 재생 중 공격 타이밍에 발동</summary>
        public event Action AttackHit;

        private void Awake()
        {
            var hitState = animancer.States.GetOrCreate(animData.GetClip(HeroAnimType.Hit));
            hitState.Events(this).OnEnd = () => Play(HeroAnimType.Idle);
            var attackState = animancer.States.GetOrCreate(animData.GetClip(HeroAnimType.Attack));
            attackState.Events(this).Add(animData.AttackTiming, OnAttackHit);
        }

        private void OnDestroy()
        {
            _damageFlashTween?.Kill();
            _victoryTween?.Kill();
        }

        public AnimancerState Play(HeroAnimType animType)
        {
            var clip = animData.GetClip(animType);
            if (clip == null) throw new InvalidOperationException($"Animation Clip(Type[{animType}]) is not found.");

            CurAnimType = animType;
            _curState = animancer.Play(clip);
            return _curState;
        }

        public void FlashRed()
        {
            _damageFlashTween?.Kill();
            _damageFlashTween = bodySr.DOColor(Color.white, visualConfig.DamageFlashDuration).From(Color.red).Play();
        }

        public YieldInstruction PlayDie()
        {
            Play(HeroAnimType.Idle);
            _curState.IsPlaying = false;

            var config = visualConfig.DieAnimConfig;
            var sequence = DOTween.Sequence()
                .Append(transform.DOMoveX(config.flyXOffset, config.flyDuration).SetRelative()
                    .SetEase(config.flyXEase))
                .Join(DOTween.To(() => 0f,
                    t => bodyTr.localPosition = new Vector3(0f, config.flyYOffsetCurve.Evaluate(t), 0f), 1f,
                    config.duration))
                .Join(bodyTr.DORotate(new Vector3(0f, 0f, config.rotateSpeed) * config.duration, config.duration,
                        RotateMode.FastBeyond360)
                    .SetEase(Ease.Linear))
                .Join(bodyTr.DOScale(Vector3.zero, config.duration).SetEase(config.scaleEase))
                .Join(shadowTr.DOScale(Vector3.zero, config.duration).SetEase(config.scaleEase))
                .Play();
            return sequence.WaitForCompletion();
        }

        public void PlayVictory(HeroVictoryType type)
        {
            _victoryTween?.Kill();
            _victoryOriginY = bodyTr.localPosition.y;

            Play(type == HeroVictoryType.SpinJump ? HeroAnimType.Victory : HeroAnimType.Idle);

            _victoryTween = bodyTr
                .DOLocalMoveY(visualConfig.VictoryJumpHeight, visualConfig.VictoryJumpDuration)
                .SetRelative().SetLoops(-1, LoopType.Yoyo).SetEase(Ease.OutQuad);
        }

        public void StopVictory()
        {
            _victoryTween?.Kill();
            _victoryTween = null;
            bodyTr.DOLocalMoveY(_victoryOriginY, visualConfig.VictoryJumpDuration * 0.5f).SetEase(Ease.InQuad);
            Play(HeroAnimType.Idle);
        }

        private void OnAttackHit()
        {
            AttackHit?.Invoke();
        }
    }
}