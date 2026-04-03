using System;
using UnityEngine;
using TwoDirection = TaeBoMiCache.TwoDirection;

namespace SOSG.Monster.Overlord
{
    public class OverlordAnimationController : MonoBehaviour
    {
        [SerializeField] private Animator ani;
        private TwoDirection _dir;

        private void Awake()
        {
            _dir = TwoDirection.Right;
        }

        public void SetDirection(TwoDirection dir) => _dir = dir;

        public void SetMove(bool value)
        {
            ani.SetBool(AnimatorCache.IsMove, value);
        }

        public void Play(AniType aniType)
        {
        }

        public enum AniType
        {
            Idle,
            Move,
            Summon,
            Embarrassment,
            Die,
        }
    }
}