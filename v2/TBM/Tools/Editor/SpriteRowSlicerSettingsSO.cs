using System;
using System.Collections.Generic;
using UnityEngine;

namespace TBM.Tools.Editor
{
    [CreateAssetMenu(fileName = "SpriteRowSlicerSettings", menuName = "TBM/Tools/Sprite Row Slicer Settings")]
    public class SpriteRowSlicerSettingsSO : ScriptableObject
    {
        [Serializable]
        public class RowData
        {
            public bool   enabled;
            public string label = "";
        }

        public int          prefixPresetIndex;
        public string       customPrefix  = "";
        public List<string> labelPresets  = new() { "FrontRight", "FrontLeft", "BackRight", "BackLeft", "Right", "Left", "Front", "Back" };
        public List<RowData> rows         = new();
    }
}
