using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MaouSuika.Gameplay
{
    [Serializable]
    public class SoulOrbMonsterSettings
    {
        [Tooltip("x: Random Value, y: duration")]
        [SerializeField] private AnimationCurve idleDurationCurve;
        [Tooltip("x: Random Value, y: duration")]
        [SerializeField] private AnimationCurve moveDurationCurve;

        [SerializeField] private float hitImpactThreshold;
        [SerializeField] private Vector2 moveRotationSpeedRange;

        [Tooltip("shell 대비 몬스터 크기 비율")]
        [SerializeField] private float fillFactor;

        public float HitImpactThreshold => hitImpactThreshold;
        public float FillFactor => fillFactor;

        public float SampleIdleDuration() => idleDurationCurve.Evaluate(Random.value);

        public float SampleMoveDuration() => moveDurationCurve.Evaluate(Random.value);

        public float SampleMoveRotationSpeed() => Random.Range(moveRotationSpeedRange.x, moveRotationSpeedRange.y);
    }
}