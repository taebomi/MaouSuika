using Animancer;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Battle
{
    [CreateAssetMenu(fileName = "HeroAnimData", menuName = "Maou Suika/Battle/Hero/Anim Data")]
    public partial class HeroAnimDataSO : ScriptableObject
    {
        [SerializeField] private AnimationClip idle;
        [SerializeField] private AnimationClip move;
        [SerializeField] private AnimationClip attack;
        [SerializeField] private AnimationClip hit;

        [field: SerializeField] public float AttackTiming { get; private set; }

        public AnimationClip GetClip(HeroAnimType type) => type switch
        {
            HeroAnimType.Idle => idle,
            HeroAnimType.Move => move,
            HeroAnimType.Attack => attack,
            HeroAnimType.Hit => hit,
            _ => null
        };
    }
}