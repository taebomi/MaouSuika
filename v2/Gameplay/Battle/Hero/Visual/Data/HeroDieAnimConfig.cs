using System;
using DG.Tweening;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Battle
{
    [Serializable]
    public struct HeroDieAnimConfig
    {
        public float flyXOffset;
        public Ease flyXEase;
        public AnimationCurve flyYOffsetCurve;
        public float flyDuration;
        public float rotateSpeed;
        public Ease scaleEase;
        public float duration;
    }
}