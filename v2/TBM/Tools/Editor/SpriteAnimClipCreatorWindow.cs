using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace TBM.Tools.Editor
{
    public class SpriteAnimClipCreatorWindow : EditorWindow
    {
        private static GUIStyle CenterMiniLabel => _centerMiniLabel ??=
            new GUIStyle(EditorStyles.miniLabel) { alignment = TextAnchor.MiddleCenter };
        private static GUIStyle _centerMiniLabel;

        private float FixedColsTotal =>
            _settings.textureColW + _settings.toggleColW + _settings.frameColW +
            _settings.fpsColW + _settings.loopColW + _settings.statusColW + 28f;

        private float NameColW => Mathf.Max(80f, position.width - FixedColsTotal);

        // ─── Data ─────────────────────────────────────────────────────────────

        private class ClipEntry
        {
            public string       Name;
            public List<Sprite> Sprites;
            public int          Fps;
            public bool         Loop;
            public bool         Enabled = true;
            public bool         Exists;
        }

        private class TextureEntry
        {
            public Texture2D       Texture;
            public string          Path;
            public List<ClipEntry> Clips = new();
        }

        // ─── State ────────────────────────────────────────────────────────────

        private SpriteAnimClipCreatorSettingsSO _settings;
        private List<TextureEntry>              _entries = new();
        private Vector2                         _scrollPos;
        private bool                            _showPresets;

        // ─── Lifecycle ────────────────────────────────────────────────────────

        [MenuItem("TBM/Tools/Sprite Anim Clip Creator")]
        public static void ShowWindow() => GetWindow<SpriteAnimClipCreatorWindow>("Anim Clip Creator");

        private void OnEnable()          { LoadSettings(); RefreshEntries(); }
        private void OnSelectionChange() { RefreshEntries(); Repaint(); }

        // ─── Settings ─────────────────────────────────────────────────────────

        private void LoadSettings()
        {
            var guids = AssetDatabase.FindAssets("t:SpriteAnimClipCreatorSettingsSO");
            if (guids.Length > 0)
            {
                _settings = AssetDatabase.LoadAssetAtPath<SpriteAnimClipCreatorSettingsSO>(
                    AssetDatabase.GUIDToAssetPath(guids[0]));
                return;
            }

            _settings = CreateInstance<SpriteAnimClipCreatorSettingsSO>();
            if (!AssetDatabase.IsValidFolder("Assets/Editor"))
                AssetDatabase.CreateFolder("Assets", "Editor");
            AssetDatabase.CreateAsset(_settings, "Assets/Editor/SpriteAnimClipCreatorSettings.asset");
            AssetDatabase.SaveAssets();
        }

        private void SaveSettings()
        {
            EditorUtility.SetDirty(_settings);
            AssetDatabase.SaveAssets();
        }

        // ─── Entry Refresh ────────────────────────────────────────────────────

        private void RefreshEntries()
        {
            var selected = Selection.GetFiltered<Texture2D>(SelectionMode.Assets);
            var existing = _entries.ToDictionary(e => e.Path);
            _entries.Clear();

            foreach (var tex in selected)
            {
                var path = AssetDatabase.GetAssetPath(tex);
                if (existing.TryGetValue(path, out var entry))
                {
                    RefreshClips(entry);
                    _entries.Add(entry);
                    continue;
                }

                var newEntry = new TextureEntry { Texture = tex, Path = path };
                RefreshClips(newEntry);
                _entries.Add(newEntry);
            }
        }

        private void RefreshClips(TextureEntry entry)
        {
            var sprites = AssetDatabase.LoadAllAssetsAtPath(entry.Path).OfType<Sprite>().ToList();
            if (sprites.Count == 0) { entry.Clips.Clear(); return; }

            var groups = new Dictionary<string, List<Sprite>>();
            foreach (var sprite in sprites)
            {
                var key = Regex.Replace(sprite.name, @"_\d+$", "");
                if (!groups.ContainsKey(key)) groups[key] = new();
                groups[key].Add(sprite);
            }

            foreach (var kvp in groups)
                kvp.Value.Sort((a, b) => GetTrailingIndex(a.name).CompareTo(GetTrailingIndex(b.name)));

            var existingClips = entry.Clips.ToDictionary(c => c.Name);
            entry.Clips.Clear();

            foreach (var kvp in groups.OrderBy(g => g.Key))
            {
                if (existingClips.TryGetValue(kvp.Key, out var ex))
                {
                    ex.Sprites = kvp.Value;
                    ex.Exists  = ClipExists(entry, ex);
                    entry.Clips.Add(ex);
                    continue;
                }

                var (fps, loop) = ResolvePreset(kvp.Key);
                var clip = new ClipEntry { Name = kvp.Key, Sprites = kvp.Value, Fps = fps, Loop = loop };
                clip.Exists = ClipExists(entry, clip);
                entry.Clips.Add(clip);
            }
        }

        private (int fps, bool loop) ResolvePreset(string clipName)
        {
            foreach (var p in _settings.fpsPresets)
            {
                if (!string.IsNullOrEmpty(p.keyword) &&
                    clipName.IndexOf(p.keyword, System.StringComparison.OrdinalIgnoreCase) >= 0)
                    return (p.fps, p.loop);
            }
            return (_settings.defaultFps, _settings.defaultLoop);
        }

        private bool ClipExists(TextureEntry tex, ClipEntry clip) =>
            AssetDatabase.LoadAssetAtPath<AnimationClip>($"{GetOutputFolder(tex)}/{clip.Name}.anim") != null;

        private string GetOutputFolder(TextureEntry tex) =>
            Path.GetDirectoryName(tex.Path)?.Replace('\\', '/') + "/" + _settings.outputFolderName;

        private static int GetTrailingIndex(string name)
        {
            var m = Regex.Match(name, @"_(\d+)$");
            return m.Success ? int.Parse(m.Groups[1].Value) : 0;
        }

        // ─── GUI ──────────────────────────────────────────────────────────────

        private void OnGUI()
        {
            GUILayout.Label("Sprite Anim Clip Creator", EditorStyles.boldLabel);
            DrawSeparator();
            DrawGlobalSettings();
            DrawSeparator();

            if (_entries.Count == 0)
            {
                EditorGUILayout.HelpBox("프로젝트 창에서 스프라이트 텍스처를 선택하세요.", MessageType.Info);
                return;
            }

            DrawTableHeader();

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
            for (var i = 0; i < _entries.Count; i++)
            {
                DrawTextureEntry(_entries[i]);
                if (i < _entries.Count - 1)
                {
                    GUILayout.Space(2);
                    DrawSeparatorLight();
                    GUILayout.Space(2);
                }
            }
            EditorGUILayout.EndScrollView();

            DrawSeparator();
            DrawCreateButton();
        }

        private void DrawGlobalSettings()
        {
            GUILayout.Label("기본 설정", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("기본 FPS", GUILayout.Width(56));
            _settings.defaultFps  = EditorGUILayout.IntField(_settings.defaultFps, GUILayout.Width(40));
            GUILayout.Space(10);
            EditorGUILayout.LabelField("기본 반복", GUILayout.Width(58));
            _settings.defaultLoop = EditorGUILayout.Toggle(_settings.defaultLoop, GUILayout.Width(20));
            GUILayout.Space(10);
            EditorGUILayout.LabelField("저장 폴더명", GUILayout.Width(64));
            _settings.outputFolderName = EditorGUILayout.TextField(_settings.outputFolderName, GUILayout.Width(100));
            GUILayout.Space(10);
            EditorGUILayout.LabelField("SR 경로", GUILayout.Width(46));
            _settings.spriteRendererPath = EditorGUILayout.TextField(_settings.spriteRendererPath, GUILayout.Width(100));
            EditorGUILayout.EndHorizontal();

            if (EditorGUI.EndChangeCheck()) SaveSettings();

            GUILayout.Space(4);

            EditorGUILayout.BeginHorizontal();
            _showPresets = EditorGUILayout.Foldout(_showPresets, "FPS 프리셋", true);
            if (GUILayout.Button("프리셋 재적용", EditorStyles.miniButton, GUILayout.Width(80)))
                ApplyPresetsToAll();
            if (GUILayout.Button("설정 관리", EditorStyles.miniButton, GUILayout.Width(64)))
                Selection.activeObject = _settings;
            EditorGUILayout.EndHorizontal();

            if (!_showPresets) return;

            GUILayout.Space(4);
            DrawPresetsGrid();
        }

        private void DrawPresetsGrid()
        {
            var cols      = Mathf.Max(1, (int)((position.width - 16f) / _settings.presetBoxW));
            var removeIdx = -1;

            for (var i = 0; i < _settings.fpsPresets.Count; i++)
            {
                if (i % cols == 0) EditorGUILayout.BeginHorizontal();

                if (DrawPresetBox(_settings.fpsPresets[i]))
                    removeIdx = i;

                if (i % cols == cols - 1 || i == _settings.fpsPresets.Count - 1)
                {
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();
                }
            }

            if (removeIdx >= 0)
            {
                _settings.fpsPresets.RemoveAt(removeIdx);
                SaveSettings();
            }

            GUILayout.Space(4);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+ 프리셋 추가", EditorStyles.miniButton, GUILayout.Width(90)))
            {
                _settings.fpsPresets.Add(new SpriteAnimClipCreatorSettingsSO.AnimFpsPreset());
                SaveSettings();
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(4);
        }

        /// <returns>true면 삭제 요청</returns>
        private bool DrawPresetBox(SpriteAnimClipCreatorSettingsSO.AnimFpsPreset preset)
        {
            var remove = false;
            EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(_settings.presetBoxW - 8f));
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("키워드", GUILayout.Width(42));
            preset.keyword = EditorGUILayout.TextField(preset.keyword, GUILayout.Width(_settings.presetKeyW));
            if (GUILayout.Button("−", EditorStyles.miniButton, GUILayout.Width(20)))
                remove = true;
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("FPS", GUILayout.Width(30));
            preset.fps  = EditorGUILayout.IntField(preset.fps, GUILayout.Width(40));
            GUILayout.Space(8);
            EditorGUILayout.LabelField("반복", GUILayout.Width(28));
            preset.loop = EditorGUILayout.Toggle(preset.loop);
            EditorGUILayout.EndHorizontal();

            if (EditorGUI.EndChangeCheck()) SaveSettings();
            EditorGUILayout.EndVertical();
            return remove;
        }

        private void ApplyPresetsToAll()
        {
            foreach (var entry in _entries)
                foreach (var clip in entry.Clips)
                    (clip.Fps, clip.Loop) = ResolvePreset(clip.Name);
        }

        // ─── Table ────────────────────────────────────────────────────────────

        private void DrawTableHeader()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("텍스처",  EditorStyles.miniLabel,  GUILayout.Width(_settings.textureColW));
            GUILayout.Label("생성",    CenterMiniLabel,          GUILayout.Width(_settings.toggleColW));
            GUILayout.Label("클립명",  EditorStyles.miniLabel,  GUILayout.Width(NameColW));
            GUILayout.Label("프레임",  CenterMiniLabel,          GUILayout.Width(_settings.frameColW));
            GUILayout.Label("FPS",    EditorStyles.miniLabel,  GUILayout.Width(_settings.fpsColW));
            GUILayout.Label("반복",    CenterMiniLabel,          GUILayout.Width(_settings.loopColW));
            GUILayout.Label("상태",    EditorStyles.miniLabel);
            EditorGUILayout.EndHorizontal();
            DrawSeparatorLight();
        }

        private void DrawTextureEntry(TextureEntry entry)
        {
            for (var i = 0; i < entry.Clips.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();

                if (i == 0)
                    EditorGUILayout.ObjectField(entry.Texture, typeof(Texture2D), false,
                        GUILayout.Width(_settings.textureColW));
                else
                    GUILayout.Space(_settings.textureColW + 4f);

                DrawClipRow(entry.Clips[i]);
                EditorGUILayout.EndHorizontal();
            }
        }

        private void DrawClipRow(ClipEntry clip)
        {
            // 체크박스 중앙 정렬
            var toggleRect = GUILayoutUtility.GetRect(_settings.toggleColW, EditorGUIUtility.singleLineHeight,
                GUIStyle.none, GUILayout.Width(_settings.toggleColW));
            toggleRect.x    += (_settings.toggleColW - 16f) * 0.5f;
            toggleRect.width = 16f;
            clip.Enabled = EditorGUI.Toggle(toggleRect, clip.Enabled);
            GUI.enabled  = clip.Enabled;

            clip.Name = EditorGUILayout.TextField(clip.Name, GUILayout.Width(NameColW));
            GUILayout.Label($"{clip.Sprites.Count}f", CenterMiniLabel, GUILayout.Width(_settings.frameColW));
            clip.Fps  = EditorGUILayout.IntField(clip.Fps, GUILayout.Width(_settings.fpsColW));

            // 반복 토글 중앙 정렬
            var loopRect = GUILayoutUtility.GetRect(_settings.loopColW, EditorGUIUtility.singleLineHeight,
                GUIStyle.none, GUILayout.Width(_settings.loopColW));
            loopRect.x    += (_settings.loopColW - 16f) * 0.5f;
            loopRect.width = 16f;
            clip.Loop = EditorGUI.Toggle(loopRect, clip.Loop);

            if (clip.Exists)
            {
                var style = new GUIStyle(EditorStyles.miniLabel)
                    { normal = { textColor = new Color(1f, 0.7f, 0.2f) } };
                GUILayout.Label("⚠ 덮어쓰기", style, GUILayout.Width(_settings.statusColW));
            }
            else
            {
                GUILayout.Label("✓ 신규", EditorStyles.miniLabel, GUILayout.Width(_settings.statusColW));
            }

            GUI.enabled = true;
        }

        // ─── Create Button ────────────────────────────────────────────────────

        private void DrawCreateButton()
        {
            var total     = _entries.SelectMany(e => e.Clips).Count(c => c.Enabled);
            var overwrite = _entries.SelectMany(e => e.Clips).Count(c => c.Enabled && c.Exists);

            GUI.enabled         = total > 0;
            GUI.backgroundColor = overwrite > 0 ? new Color(1f, 0.85f, 0.5f) : new Color(0.7f, 1f, 0.7f);

            var label = overwrite > 0
                ? $"Create  {total}개 클립 생성  ({overwrite}개 덮어쓰기)"
                : $"Create  {total}개 클립 생성";

            if (GUILayout.Button(label, GUILayout.Height(36)))
                CreateClips();

            GUI.backgroundColor = Color.white;
            GUI.enabled         = true;
        }

        // ─── Create ───────────────────────────────────────────────────────────

        private void CreateClips()
        {
            AssetDatabase.StartAssetEditing();
            var created = 0;
            try
            {
                foreach (var entry in _entries)
                {
                    var folder = GetOutputFolder(entry);
                    EnsureFolder(folder);
                    foreach (var clip in entry.Clips.Where(c => c.Enabled))
                    {
                        CreateClipAsset(clip, folder, _settings.spriteRendererPath);
                        created++;
                    }
                }
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
                AssetDatabase.Refresh();
            }

            foreach (var entry in _entries) RefreshClips(entry);
            Debug.Log($"<color=green>AnimationClip 생성 완료</color> → {created}개");
        }

        private static void CreateClipAsset(ClipEntry clipEntry, string folder, string srPath)
        {
            var clip = new AnimationClip { frameRate = clipEntry.Fps };

            var clipSettings = AnimationUtility.GetAnimationClipSettings(clip);
            clipSettings.loopTime = clipEntry.Loop;
            AnimationUtility.SetAnimationClipSettings(clip, clipSettings);

            var binding   = EditorCurveBinding.PPtrCurve(srPath, typeof(SpriteRenderer), "m_Sprite");
            var keyframes = new ObjectReferenceKeyframe[clipEntry.Sprites.Count];
            for (var i = 0; i < clipEntry.Sprites.Count; i++)
                keyframes[i] = new ObjectReferenceKeyframe
                {
                    time  = i / (float)clipEntry.Fps,
                    value = clipEntry.Sprites[i],
                };
            AnimationUtility.SetObjectReferenceCurve(clip, binding, keyframes);

            var assetPath = $"{folder}/{clipEntry.Name}.anim";
            var existing  = AssetDatabase.LoadAssetAtPath<AnimationClip>(assetPath);
            if (existing != null)
            {
                EditorUtility.CopySerialized(clip, existing);
                EditorUtility.SetDirty(existing);
            }
            else
            {
                AssetDatabase.CreateAsset(clip, assetPath);
            }
        }

        private static void EnsureFolder(string folderPath)
        {
            if (AssetDatabase.IsValidFolder(folderPath)) return;
            var parent  = Path.GetDirectoryName(folderPath)?.Replace('\\', '/');
            var dirName = Path.GetFileName(folderPath);
            if (!string.IsNullOrEmpty(parent) && !string.IsNullOrEmpty(dirName))
                AssetDatabase.CreateFolder(parent, dirName);
        }

        // ─── Helpers ──────────────────────────────────────────────────────────

        private static void DrawSeparator()
        {
            GUILayout.Space(4);
            EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.3f, 0.3f, 0.3f));
            GUILayout.Space(4);
        }

        private static void DrawSeparatorLight()
        {
            EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.25f, 0.25f, 0.25f));
        }
    }
}
