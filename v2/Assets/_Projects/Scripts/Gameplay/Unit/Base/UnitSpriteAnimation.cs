using System;
using UnityEngine;

namespace MaouSuika.Gameplay
{
    [Serializable]
    public readonly struct UnitSpriteAnimation
    {
        public readonly Sprite[] Frames;
        public readonly int FPS;
        public readonly bool Loop;
        public readonly bool FlipX;

        public float Duration => Frames.Length / (float)FPS;

        public UnitSpriteAnimation(Sprite[] frames, int fps, bool loop, bool flipX)
        {
            Frames = frames;
            FPS = fps;
            Loop = loop;
            FlipX = flipX;
        }
    }
}