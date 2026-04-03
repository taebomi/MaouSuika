using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    public abstract class MergeEffectBaseConfigSO<TProfile> : ScriptableObject
    {
        [SerializeField] private TProfile[] colorProfiles;

        [NonSerialized] private Dictionary<MergeEffectColor, TProfile> _colorProfileDict;


        protected abstract MergeEffectColor GetColorType(TProfile profile);

        public TProfile GetProfile(MergeEffectColor color)
        {
            if (_colorProfileDict == null)
            {
                if (colorProfiles == null || colorProfiles.Length == 0)
                {
                    throw new InvalidOperationException($"Color Profile[{name}] is empty.");
                }

                _colorProfileDict = colorProfiles.ToDictionary(GetColorType, p => p);
            }

            if (_colorProfileDict.TryGetValue(color, out var profile)) return profile;

            Debug.LogError($"Color[{color}] Profile is not found.");
            return colorProfiles[0];
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            _colorProfileDict = null;
        }
#endif
    }
}