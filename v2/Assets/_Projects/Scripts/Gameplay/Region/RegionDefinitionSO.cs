using System;
using FMODUnity;
using MaouSuika.Core;
using UnityEngine;

namespace MaouSuika.Gameplay
{
    [CreateAssetMenu(fileName = "RegionDefinitionSO", menuName = MenuPath.Gameplay.Region + "Definition")]
    public class RegionDefinitionSO : ScriptableObject
    {
        [Serializable]
        public class PuzzlePresentationSettings
        {
            [SerializeField] private PuzzleRegionPresentation prefab;
            [SerializeField, Min(0f)] private float globalLightIntensity;

            public PuzzleRegionPresentation Prefab => prefab;
            public float GlobalLightIntensity => globalLightIntensity;
        }

        [SerializeField] private string id;
        [SerializeField] private EventReference bgm;
        [SerializeField] private PuzzlePresentationSettings puzzle = new();

        public string Id => id;
        public EventReference Bgm => bgm;
        public PuzzlePresentationSettings Puzzle => puzzle;
    }
}