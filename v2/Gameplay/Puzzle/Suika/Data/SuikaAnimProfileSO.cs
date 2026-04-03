using System.Linq;
using AYellowpaper.SerializedCollections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    [CreateAssetMenu(fileName = "SuikaAnimProfileSO", menuName = "Maou Suika/Puzzle/Suika/Anim Profile")]
    public class SuikaAnimProfileSO : ScriptableObject
    {
        [Tooltip("x : random value, y : time")]
        [SerializeField] private AnimationCurve idleTimeCurve;
        [Tooltip("x : random value, y : time")]
        [SerializeField] private AnimationCurve moveTimeCurve;

        [field: SerializeField] public float HitCooldown { get; private set; }

        [MinMaxSlider(0f, 360f, true)]
        [SerializeField] private Vector2 moveRotationSpeedRange;

        public float GetRandomIdleTime()
        {
            return idleTimeCurve.Evaluate(Random.value);
        }

        public float GetRandomMoveTime()
        {
            return moveTimeCurve.Evaluate(Random.value);
        }

        public float GetRandomMoveRotationSpeed()
        {
            return Random.Range(moveRotationSpeedRange.x, moveRotationSpeedRange.y);
        }
    }
}