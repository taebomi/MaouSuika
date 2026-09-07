#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace MaouSuika.Gameplay
{
    public partial class MonsterSpriteAnimationSetSO
    {
        private const string SPRITES_FOLDER_NAME = "Sprites";

        private static readonly MonsterAnimationState[] STATES =
        {
            MonsterAnimationState.Idle,
            MonsterAnimationState.Move,
            MonsterAnimationState.Attack,
            MonsterAnimationState.Hit,
            MonsterAnimationState.Die,
        };

        private static readonly Regex SPRITE_NAME_PATTERN = new(
            @"_(Idle|Move|Attack|Hit|Die)(?:_(Left|Right))?_(\d+)$",
            RegexOptions.Compiled | RegexOptions.CultureInvariant);

        [Button(ButtonSizes.Large)]
        private void RefreshSpritesAndApplyPreset()
        {
            if (!TryFindGlobalPreset(out var preset)) return;

            var assetPath = AssetDatabase.GetAssetPath(this);
            var monsterFolder = Path.GetDirectoryName(assetPath)?.Replace('\\', '/');
            var spritesFolder = $"{monsterFolder}/{SPRITES_FOLDER_NAME}";

            if (string.IsNullOrEmpty(monsterFolder) || !AssetDatabase.IsValidFolder(spritesFolder))
            {
                Debug.LogError($"[{name}] Sprites folder does not exist: {spritesFolder}", this);
                return;
            }

            var framesByState = STATES.ToDictionary(state => state, _ => new DirectionFrames());
            var ignoredNames = new List<string>();
            var errors = new List<string>();

            foreach (var sprite in LoadSpritesDirectlyUnder(spritesFolder))
            {
                var match = SPRITE_NAME_PATTERN.Match(sprite.name);
                if (!match.Success)
                {
                    ignoredNames.Add(sprite.name);
                    continue;
                }

                var state = Enum.Parse<MonsterAnimationState>(match.Groups[1].Value);
                var facing = match.Groups[2].Success
                    ? Enum.Parse<UnitFacing>(match.Groups[2].Value)
                    : UnitFacing.Right;
                var frameIndex = int.Parse(match.Groups[3].Value);
                var frames = framesByState[state].Get(facing);

                if (!frames.TryAdd(frameIndex, sprite))
                {
                    errors.Add($"Duplicate frame: {state}/{facing}/{frameIndex}");
                }
            }

            foreach (var state in STATES)
            {
                var stateFrames = framesByState[state];
                ValidateFrameSequence(state, UnitFacing.Left, stateFrames.Left, errors);
                ValidateFrameSequence(state, UnitFacing.Right, stateFrames.Right, errors);

                if (preset.Get(state) == null)
                {
                    errors.Add($"Preset is missing settings for {state}.");
                }
            }

            if (framesByState.Values.All(frames => frames.Left.Count == 0 && frames.Right.Count == 0))
            {
                errors.Add("No sprites matched the required naming rule.");
            }

            if (errors.Count > 0)
            {
                Debug.LogError($"[{name}] Refresh aborted.\n- {string.Join("\n- ", errors)}", this);
                return;
            }

            Undo.RecordObject(this, "Refresh Monster Sprite Animation Set");

            idle ??= new UnitDirectionalSpriteAnimation();
            move ??= new UnitDirectionalSpriteAnimation();
            attack ??= new UnitDirectionalSpriteAnimation();
            hit ??= new UnitDirectionalSpriteAnimation();
            die ??= new UnitDirectionalSpriteAnimation();

            Apply(idle, MonsterAnimationState.Idle, framesByState, preset);
            Apply(move, MonsterAnimationState.Move, framesByState, preset);
            Apply(attack, MonsterAnimationState.Attack, framesByState, preset);
            Apply(hit, MonsterAnimationState.Hit, framesByState, preset);
            Apply(die, MonsterAnimationState.Die, framesByState, preset);

            EditorUtility.SetDirty(this);

            var result = new StringBuilder($"[{name}] Refreshed from {spritesFolder}.");
            foreach (var state in STATES)
            {
                var frames = framesByState[state];
                var statePreset = preset.Get(state);
                result.Append($"\n- {state}: L {frames.Left.Count}, R {frames.Right.Count}, " +
                    $"{statePreset.Fps} FPS, Loop {statePreset.Loop}");
            }

            if (ignoredNames.Count > 0)
            {
                result.Append($"\n- Ignored sprites: {ignoredNames.Count}");
            }

            Debug.Log(result.ToString(), this);
        }

        private bool TryFindGlobalPreset(out MonsterSpriteAnimationPresetSO globalPreset)
        {
            var presetPaths = AssetDatabase
                .FindAssets($"t:{nameof(MonsterSpriteAnimationPresetSO)}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .ToArray();

            if (presetPaths.Length != 1)
            {
                var found = presetPaths.Length == 0
                    ? "None"
                    : $"\n- {string.Join("\n- ", presetPaths)}";

                Debug.LogError(
                    $"[{name}] The project must contain exactly one " +
                    $"{nameof(MonsterSpriteAnimationPresetSO)} asset. Found: {presetPaths.Length}\n{found}",
                    this);

                globalPreset = null;
                return false;
            }

            globalPreset = AssetDatabase.LoadAssetAtPath<MonsterSpriteAnimationPresetSO>(presetPaths[0]);
            if (globalPreset != null) return true;

            Debug.LogError($"[{name}] Failed to load the global preset: {presetPaths[0]}", this);
            return false;
        }

        private static IEnumerable<Sprite> LoadSpritesDirectlyUnder(string spritesFolder)
        {
            return AssetDatabase.FindAssets("t:Texture2D", new[] { spritesFolder })
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(path => GetParentFolder(path) == spritesFolder)
                .Distinct()
                .SelectMany(AssetDatabase.LoadAllAssetsAtPath)
                .OfType<Sprite>();
        }

        private static string GetParentFolder(string assetPath)
        {
            return Path.GetDirectoryName(assetPath)?.Replace('\\', '/');
        }

        private static void ValidateFrameSequence(MonsterAnimationState state, UnitFacing facing,
            SortedDictionary<int, Sprite> frames, ICollection<string> errors)
        {
            if (frames.Count == 0) return;

            var expectedIndex = 0;
            foreach (var frameIndex in frames.Keys)
            {
                if (frameIndex != expectedIndex)
                {
                    errors.Add($"Missing frame: {state}/{facing}/{expectedIndex}");
                    return;
                }

                expectedIndex++;
            }
        }

        private void Apply(UnitDirectionalSpriteAnimation animation, MonsterAnimationState state,
            IReadOnlyDictionary<MonsterAnimationState, DirectionFrames> framesByState,
            MonsterSpriteAnimationPresetSO globalPreset)
        {
            var frames = framesByState[state];
            animation.ApplyPreset(frames.Left.Values.ToArray(), frames.Right.Values.ToArray(),
                globalPreset.Get(state));
        }

        private sealed class DirectionFrames
        {
            public readonly SortedDictionary<int, Sprite> Left = new();
            public readonly SortedDictionary<int, Sprite> Right = new();

            public SortedDictionary<int, Sprite> Get(UnitFacing facing)
            {
                return facing switch
                {
                    UnitFacing.Left => Left,
                    UnitFacing.Right => Right,
                    _ => throw new ArgumentOutOfRangeException(nameof(facing), facing, null),
                };
            }
        }
    }
}

#endif
