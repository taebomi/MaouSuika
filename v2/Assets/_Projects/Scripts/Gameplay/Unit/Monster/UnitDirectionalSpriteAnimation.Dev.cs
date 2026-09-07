#if UNITY_EDITOR

using UnityEngine;

namespace MaouSuika.Gameplay
{
    public partial class UnitDirectionalSpriteAnimation
    {
        internal void ApplyPreset(Sprite[] leftFrames, Sprite[] rightFrames,
            MonsterSpriteAnimationPresetSO.StatePreset preset)
        {
            left = leftFrames;
            right = rightFrames;
            fps = preset.Fps;
            loop = preset.Loop;
        }
    }
}

#endif
