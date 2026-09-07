using System;
using MaouSuika.Core;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MaouSuika.Gameplay
{
    [Serializable]
    public class SoulOrbMonster
    {
        [SerializeField] private Transform anchor;
        [SerializeField] private UnitVisual visual;

        private SoulOrbMonsterSettings _settings;
        private MonsterSpriteAnimationSetSO _animationSet;

        private readonly GaitSampler _moveGaitSampler = new();

        private UnitFacing _facing;

        private MonsterAnimationState _baseState;
        private float _baseStateRemaining;

        private MonsterAnimationState _playingState;

        private float _moveRotationSpeed;

        private float _hitDuration;
        private float _hitRemaining;

        public void Initialize(SoulOrbMonsterSettings settings)
        {
            _settings = settings;

            visual.SetShadowVisible(false);
        }


        public void Setup(MonsterDataSO data, float shellRadius, int tier)
        {
            _baseStateRemaining = 0f;
            _hitRemaining = 0f;
            _moveRotationSpeed = 0f;

            _facing = Random.value < 0.5f ? UnitFacing.Left : UnitFacing.Right;

            _animationSet = data.spriteAnimationSet;
            _hitDuration = _animationSet.Resolve(MonsterAnimationState.Hit, _facing).Duration;
            var moveDuration = _animationSet.Resolve(MonsterAnimationState.Move, _facing).Duration;

            _moveGaitSampler.Setup(_animationSet.MoveGait, moveDuration);

            visual.ResetStates();
            FitVisual(data.GetFitCenter(_facing), data.fitRadius, shellRadius);
            visual.SetSortingOrder(tier);

            anchor.localRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));

            EnterIdle();
        }

        private void FitVisual(Vector2 fitCenter, float fitRadius, float shellRadius)
        {
            var scale = Mathf.Min(shellRadius * _settings.FillFactor / fitRadius, 1f);
            visual.transform.localScale = new Vector3(scale, scale, 1f);
            visual.transform.localPosition = -fitCenter * scale;
        }

        public void TickVisuals(float deltaTime)
        {
            visual.Tick(deltaTime);

            if (_playingState is MonsterAnimationState.Hit) TickHit(deltaTime);
            else TickBaseState(deltaTime);
        }

        public void Rotate(float delta)
        {
            anchor.Rotate(0f, 0f, delta);
        }

        public void SetSortingLayerID(int id)
        {
            visual.SetSortingLayerID(id);
        }

        public void SetColor(Color color)
        {
            visual.SetColor(color);
        }

        public void OnCollided(float impactSpeed)
        {
            if (impactSpeed < _settings.HitImpactThreshold) return;

            _hitRemaining = _hitDuration;

            if (_playingState is MonsterAnimationState.Hit) return;

            Play(MonsterAnimationState.Hit);
        }

        private void TickHit(float deltaTime)
        {
            _hitRemaining -= deltaTime;

            if (_hitRemaining > 0f) return;

            Play(_baseState);
        }

        private void TickBaseState(float deltaTime)
        {
            _baseStateRemaining -= deltaTime;

            if (_baseStateRemaining <= 0f)
            {
                ToggleBaseState();
                return;
            }

            if (_baseState is not MonsterAnimationState.Move) return;

            var stepTime = _moveGaitSampler.StepTime(visual.NormalizedTime); // 연출이므로 간단하게 시각에 종속적으로 처리
            anchor.Rotate(0f, 0f, _moveRotationSpeed * stepTime);
        }

        private void ToggleBaseState()
        {
            if (_baseState is MonsterAnimationState.Idle) EnterMove();
            else EnterIdle();
        }

        private void EnterIdle()
        {
            _baseState = MonsterAnimationState.Idle;
            _baseStateRemaining = _settings.SampleIdleDuration();

            Play(_baseState);
        }

        private void EnterMove()
        {
            _baseState = MonsterAnimationState.Move;
            var rotationSign = -_facing.ToSign();
            _moveRotationSpeed = _settings.SampleMoveRotationSpeed() * rotationSign;
            _baseStateRemaining = _settings.SampleMoveDuration();

            Play(_baseState);
        }

        private void Play(MonsterAnimationState state)
        {
            _playingState = state;

            var animation = _animationSet.Resolve(state, _facing);
            visual.Play(animation);

            if (state is MonsterAnimationState.Move) _moveGaitSampler.Restart();
        }
    }
}