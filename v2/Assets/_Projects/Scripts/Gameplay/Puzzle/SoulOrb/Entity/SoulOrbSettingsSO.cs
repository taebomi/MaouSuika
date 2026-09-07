using System;
using FMODUnity;
using MaouSuika.Core;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MaouSuika.Gameplay
{
    [CreateAssetMenu(fileName = "SoulOrb_Settings_SO"
        , menuName = MenuPath.Gameplay.SOUL_ORB + "Settings")]
    public class SoulOrbSettingsSO : ScriptableObject
    {
        [Serializable]
        public class SpinSettings
        {
            [Tooltip("최대 파워로 발사했을 때의 회전 속도(도/초)")]
            [field: SerializeField] public float Speed { get; private set; } = 200f;
            [Tooltip("첫 충돌 이후의 감쇠량(도/초²)")]
            [field: SerializeField] public float StopDelay { get; private set; } = 6000f;
        }

        [field: SerializeField] public SpinSettings Spin { get; private set; }

        [SerializeField] private EventReference collisionSfx;
        [SerializeField] private float collisionSfxMinImpact;


        [SerializeField] private SoulOrbMonsterSettings monster;

        public SoulOrbMonsterSettings Monster => monster;

        public EventReference CollisionSfx => collisionSfx;
        public float CollisionSfxMinImpact => collisionSfxMinImpact;
    }
}