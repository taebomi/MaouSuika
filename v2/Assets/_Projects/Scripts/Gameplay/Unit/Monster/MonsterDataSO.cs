using MaouSuika.Core;
using UnityEngine;

namespace MaouSuika.Gameplay
{
    [CreateAssetMenu(fileName = "MonsterDataSO", menuName = MenuPath.Gameplay.Unit.MONSTER + "Data")]
    public partial class MonsterDataSO : ScriptableObject
    {
        public string id;

        public MonsterSpriteAnimationSetSO spriteAnimationSet;

        public Grade grade;
        public Sprite icon;

        public MonsterStats stats;

        public Color soulOrbColor;
        public SoulOrbPopEffectColor soulOrbPopEffectColor;
        [Tooltip("피벗(발) 기준, 몬스터의 중심")]
        [SerializeField] private Vector2 fitCenter;
        [Tooltip("피벗(발) 기준, 몬스터의 반지름")] public float fitRadius;

        [Tooltip("피벗(발) 기준, 피격 판정 중심. 투사체 조준점으로 쓴다.")]
        [SerializeField] private Vector2 hitCenter;
        [Tooltip("피격 판정 반지름. 1D 접촉 판정의 좌우 반폭이기도 하다.")] public float hitRadius;

        public Vector2 GetFitCenter(UnitFacing facing) => new(fitCenter.x * facing.ToSign(), fitCenter.y);

        public Vector2 GetHitCenter(UnitFacing facing) => new(hitCenter.x * facing.ToSign(), hitCenter.y);
    }
}