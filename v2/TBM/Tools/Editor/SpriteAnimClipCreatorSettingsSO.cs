using System;
using System.Collections.Generic;
using UnityEngine;

namespace TBM.Tools.Editor
{
    [CreateAssetMenu(fileName = "SpriteAnimClipCreatorSettings",
        menuName = "TBM/Tools/Sprite Anim Clip Creator Settings")]
    public class SpriteAnimClipCreatorSettingsSO : ScriptableObject
    {
        [Serializable]
        public class AnimFpsPreset
        {
            public string keyword = "";
            public int    fps     = 12;
            public bool   loop    = true;
        }

        public int    defaultFps          = 12;
        public bool   defaultLoop         = true;
        public string outputFolderName    = "Animations";
        public string spriteRendererPath  = "Body/Sprite";

        [Header("테이블 열 너비")]
        public float textureColW = 150f;
        public float toggleColW  = 28f;
        public float frameColW   = 34f;
        public float fpsColW     = 40f;
        public float loopColW    = 28f;
        public float statusColW  = 76f;
        public float presetBoxW  = 210f;
        public float presetKeyW  = 60f;

        public List<AnimFpsPreset> fpsPresets = new()
        {
            new AnimFpsPreset { keyword = "Idle",   fps = 8,  loop = true  },
            new AnimFpsPreset { keyword = "Move",   fps = 8,  loop = true  },
            new AnimFpsPreset { keyword = "Attack", fps = 12, loop = false },
            new AnimFpsPreset { keyword = "Hit",    fps = 12, loop = false },
        };
    }
}
