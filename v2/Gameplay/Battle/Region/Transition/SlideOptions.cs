using System;
using DG.Tweening;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Battle
{
    [Serializable]
    public struct SlideOptions
    {
        public float offset;
        public float duration;
        public float interval;
        public AnimationCurve slideInEase;
        public AnimationCurve slideOutEase;
    }
}