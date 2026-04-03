using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.U2D.Sprites;
using UnityEditorInternal;
using UnityEngine;

namespace TBM.Tools.Editor
{
    public class SpriteRowSlicerWindow : EditorWindow
    {
        // ================================================
        // Data
        // ================================================
        private class RowSetting
        {
            public bool   Enabled;
            public string Label = "";
        }

        private class TextureEntry
        {
            public Texture2D       Texture;
            public string          Path;
            public int             SpriteWidth;
            public int             SpriteHeight;
            public int             Cols;
            public int             Rows;
            public string          Prefix;
            public bool            Enabled  = true;
            public bool            Expanded = true;
            public bool            SizeAutoDetected;
            public List<RowSetting> RowOverrides = new();
        }

        private enum PrefixPreset { TextureName, Custom }

        // ================================================
        // Constants
        // ================================================
        private const string DefaultSettingsPath = "Assets/Editor/SpriteRowSlicerSettings.asset";
        private const float  PreviewSize         = 36f;

        private static readonly Color SeparatorColor  = new(0.25f, 0.25f, 0.25f);
        private static readonly Color RowEnabledColor = new(0.22f, 0.36f, 0.22f);

        // ================================================
        // State
        // ================================================
        private SpriteRowSlicerSettingsSO _settings;
        private ReorderableList           _presetList;

        private readonly List<TextureEntry> _entries     = new();
        private readonly List<RowSetting>   _rowSettings = new(); // Global (일괄)
        private int _maxRowsToShow;

        private PrefixPreset _prefixPreset = PrefixPreset.TextureName;
        private string       _customPrefix = "";

        private Vector2 _scrollPos;

        // ================================================
        // Window
        // ================================================
        [MenuItem("TBM/Tools/Sprite Row Slicer")]
        public static void ShowWindow() => GetWindow<SpriteRowSlicerWindow>("Sprite Row Slicer", true);

        private void OnEnable()
        {
            _settings = FindOrCreateSettings();
            LoadFromSettings();
            BuildPresetList();
            RefreshEntries();
        }

        private void OnSelectionChange() { RefreshEntries(); Repaint(); }

        // ================================================
        // Settings SO
        // ================================================
        private static SpriteRowSlicerSettingsSO FindOrCreateSettings()
        {
            var guids = AssetDatabase.FindAssets("t:SpriteRowSlicerSettingsSO");
            if (guids.Length > 0)
                return AssetDatabase.LoadAssetAtPath<SpriteRowSlicerSettingsSO>(
                    AssetDatabase.GUIDToAssetPath(guids[0]));

            var so = CreateInstance<SpriteRowSlicerSettingsSO>();
            if (!AssetDatabase.IsValidFolder("Assets/Editor"))
                AssetDatabase.CreateFolder("Assets", "Editor");
            AssetDatabase.CreateAsset(so, DefaultSettingsPath);
            AssetDatabase.SaveAssets();
            return so;
        }

        private void SaveToSettings()
        {
            if (_settings == null) return;
            Undo.RecordObject(_settings, "Sprite Row Slicer Settings");

            _settings.prefixPresetIndex = (int)_prefixPreset;
            _settings.customPrefix      = _customPrefix;
            _settings.rows.Clear();
            foreach (var row in _rowSettings)
                _settings.rows.Add(new SpriteRowSlicerSettingsSO.RowData
                    { enabled = row.Enabled, label = row.Label });

            EditorUtility.SetDirty(_settings);
        }

        private void LoadFromSettings()
        {
            if (_settings == null) return;
            _prefixPreset = (PrefixPreset)Mathf.Clamp(_settings.prefixPresetIndex, 0, 1);
            _customPrefix = _settings.customPrefix;

            _rowSettings.Clear();
            foreach (var row in _settings.rows)
                _rowSettings.Add(new RowSetting { Enabled = row.enabled, Label = row.label });
        }

        // ================================================
        // Preset List
        // ================================================
        private void BuildPresetList()
        {
            if (_settings == null) return;
            _presetList = new ReorderableList(_settings.labelPresets, typeof(string),
                draggable: true, displayHeader: true,
                displayAddButton: true, displayRemoveButton: true)
            {
                drawHeaderCallback  = r => EditorGUI.LabelField(r, "라벨 프리셋 목록"),
                drawElementCallback = (r, i, _, _) =>
                {
                    r.y += 2; r.height -= 4;
                    EditorGUI.BeginChangeCheck();
                    _settings.labelPresets[i] = EditorGUI.TextField(r, _settings.labelPresets[i]);
                    if (EditorGUI.EndChangeCheck()) EditorUtility.SetDirty(_settings);
                },
                onAddCallback    = _ => { _settings.labelPresets.Add(""); EditorUtility.SetDirty(_settings); },
                onRemoveCallback = l => { _settings.labelPresets.RemoveAt(l.index); EditorUtility.SetDirty(_settings); },
                onReorderCallback = _ => EditorUtility.SetDirty(_settings),
            };
        }

        // ================================================
        // Entry Management
        // ================================================
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
                    _entries.Add(entry);
                    continue;
                }

                var newEntry = new TextureEntry { Texture = tex, Path = path, Prefix = tex.name };
                DetectSpriteSize(newEntry);
                InitTextureRowOverrides(newEntry);
                _entries.Add(newEntry);
            }

            SyncRowSettings();
        }

        private void SyncRowSettings()
        {
            _maxRowsToShow = _entries.Count > 0
                ? _entries.Max(e => e.Rows)
                : Mathf.Max(_rowSettings.Count, 4);

            while (_rowSettings.Count < _maxRowsToShow)
                _rowSettings.Add(new RowSetting { Enabled = _rowSettings.Count == 1 });

            foreach (var entry in _entries)
                EnsureRowOverridesSize(entry);
        }

        private void InitTextureRowOverrides(TextureEntry entry)
        {
            entry.RowOverrides.Clear();
            for (var i = 0; i < entry.Rows; i++)
            {
                var global = i < _rowSettings.Count ? _rowSettings[i] : null;
                entry.RowOverrides.Add(new RowSetting
                {
                    Enabled = global?.Enabled ?? false,
                    Label   = global?.Label   ?? "",
                });
            }
        }

        private static void EnsureRowOverridesSize(TextureEntry entry)
        {
            while (entry.RowOverrides.Count < entry.Rows)
                entry.RowOverrides.Add(new RowSetting { Enabled = false });
        }

        private void ApplyGlobalToEntry(TextureEntry entry)
        {
            for (var i = 0; i < entry.RowOverrides.Count; i++)
            {
                var global = i < _rowSettings.Count ? _rowSettings[i] : null;
                entry.RowOverrides[i].Enabled = global?.Enabled ?? false;
                entry.RowOverrides[i].Label   = global?.Label   ?? "";
            }
        }

        private void ApplyGlobalToAll()
        {
            foreach (var entry in _entries)
                ApplyGlobalToEntry(entry);
        }

        private static void DetectSpriteSize(TextureEntry entry)
        {
            if (AssetImporter.GetAtPath(entry.Path) is not TextureImporter importer) return;

            if (importer.spriteImportMode == SpriteImportMode.Multiple)
            {
                var factory = new SpriteDataProviderFactories();
                factory.Init();
                var dataProvider = factory.GetSpriteEditorDataProviderFromObject(importer);
                dataProvider.InitSpriteEditorDataProvider();
                var rects = dataProvider.GetSpriteRects();

                if (rects.Length > 0)
                {
                    entry.SpriteWidth      = (int)rects[0].rect.width;
                    entry.SpriteHeight     = (int)rects[0].rect.height;
                    entry.SizeAutoDetected = true;
                    UpdateGrid(entry);
                    return;
                }
            }

            entry.SpriteWidth      = 16;
            entry.SpriteHeight     = 16;
            entry.SizeAutoDetected = false;
            UpdateGrid(entry);
        }

        private static void UpdateGrid(TextureEntry entry)
        {
            entry.Cols = entry.Texture.width  / Mathf.Max(1, entry.SpriteWidth);
            entry.Rows = entry.Texture.height / Mathf.Max(1, entry.SpriteHeight);
        }

        // ================================================
        // GUI
        // ================================================
        private void OnGUI()
        {
            _scrollPos = GUILayout.BeginScrollView(_scrollPos);
            GUILayout.Space(6);

            DrawSectionHeader("일괄 설정");
            DrawGlobalRowSettings();

            GUILayout.Space(10);

            if (_entries.Count == 0)
            {
                EditorGUILayout.HelpBox("프로젝트 창에서 텍스처를 하나 이상 선택하세요.", MessageType.Info);
                GUILayout.EndScrollView();
                return;
            }

            DrawSectionHeader($"텍스처  ({_entries.Count}개 선택됨)");
            DrawTextureList();

            GUILayout.Space(10);
            DrawSliceButton();

            GUILayout.Space(6);
            GUILayout.EndScrollView();
        }

        // ================================================
        // Global Row Settings (일괄)
        // ================================================
        private void DrawGlobalRowSettings()
        {
            GUILayout.Space(2);
            EditorGUI.BeginChangeCheck();

            // Prefix preset
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(4);
            GUILayout.Label("접두사", GUILayout.Width(40));
            _prefixPreset = (PrefixPreset)EditorGUILayout.EnumPopup(_prefixPreset, GUILayout.Width(110));
            if (_prefixPreset == PrefixPreset.Custom)
            {
                GUILayout.Space(4);
                _customPrefix = EditorGUILayout.TextField(_customPrefix);
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(4);

            // Row settings (grid)
            const float cellMinWidth = 160f;
            const float indent       = 4f;
            var availableWidth = position.width - indent - 8f;
            var cols           = Mathf.Max(1, Mathf.FloorToInt(availableWidth / cellMinWidth));
            var cellWidth      = availableWidth / cols;

            for (var r = 0; r < _maxRowsToShow; r++)
            {
                if (r % cols == 0)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(indent);
                }

                var row = _rowSettings[r];
                EditorGUILayout.BeginHorizontal(GUILayout.Width(cellWidth));
                row.Enabled = EditorGUILayout.Toggle(row.Enabled, GUILayout.Width(16));
                GUILayout.Label($"행 {r}", GUILayout.Width(32));
                row.Label = EditorGUILayout.TextField(row.Label);
                if (GUILayout.Button("▾", EditorStyles.miniButton, GUILayout.Width(22)))
                    ShowRowLabelPresetMenu(row, null);
                EditorGUILayout.EndHorizontal();

                if (r % cols == cols - 1 || r == _maxRowsToShow - 1)
                    EditorGUILayout.EndHorizontal();
            }

            if (EditorGUI.EndChangeCheck())
                SaveToSettings();

            GUILayout.Space(4);
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("전체 텍스처에 적용", EditorStyles.miniButton, GUILayout.Width(110)))
            {
                foreach (var e in _entries)
                {
                    ApplyPrefixPreset(e);
                    ApplyGlobalToEntry(e);
                }
            }
            if (GUILayout.Button("프리셋 관리", EditorStyles.miniButton, GUILayout.Width(70)))
            {
                Selection.activeObject = _settings;
                EditorGUIUtility.PingObject(_settings);
            }
            EditorGUILayout.EndHorizontal();
        }

        // ================================================
        // Per-Texture List
        // ================================================
        private void DrawTextureList()
        {
            foreach (var entry in _entries)
                DrawTextureEntry(entry);
        }

        private void DrawTextureEntry(TextureEntry entry)
        {
            // Row 1: Toggle + Foldout (파일명 전체 표시)
            var headerRect = EditorGUILayout.BeginHorizontal();
            EditorGUI.DrawRect(headerRect, new Color(0.18f, 0.18f, 0.18f));

            entry.Enabled  = EditorGUILayout.Toggle(entry.Enabled, GUILayout.Width(16));
            entry.Expanded = EditorGUILayout.Foldout(entry.Expanded, entry.Texture.name, true);

            EditorGUILayout.EndHorizontal();

            // Row 2: 크기, 접두사, 리셋 버튼
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(20);

            EditorGUILayout.ObjectField(entry.Texture, typeof(Texture2D), false, GUILayout.Width(28));

            var newW = EditorGUILayout.IntField(entry.SpriteWidth,  GUILayout.Width(32));
            GUILayout.Label("×", GUILayout.Width(10));
            var newH = EditorGUILayout.IntField(entry.SpriteHeight, GUILayout.Width(32));
            if (newW != entry.SpriteWidth || newH != entry.SpriteHeight)
            {
                entry.SpriteWidth      = Mathf.Max(1, newW);
                entry.SpriteHeight     = Mathf.Max(1, newH);
                UpdateGrid(entry);
                EnsureRowOverridesSize(entry);
                SyncRowSettings();
            }

            GUI.color = entry.SizeAutoDetected ? Color.white : Color.yellow;
            GUILayout.Label($"→ {entry.Cols}×{entry.Rows}", EditorStyles.miniLabel, GUILayout.Width(44));
            GUI.color = Color.white;

            GUI.enabled  = entry.Enabled;
            entry.Prefix = EditorGUILayout.TextField(entry.Prefix);
            if (GUILayout.Button("↺", EditorStyles.miniButton, GUILayout.Width(22)))
            {
                ApplyPrefixPreset(entry);
                ApplyGlobalToEntry(entry);
            }
            GUI.enabled = true;

            EditorGUILayout.EndHorizontal();

            // Per-row preview (expanded)
            if (entry.Expanded)
            {
                GUILayout.Space(4);
                DrawTextureRowOverrides(entry);
            }

            DrawSeparator();
        }

        private void DrawTextureRowOverrides(TextureEntry entry)
        {
            const float cellMinWidth = 190f;
            const float indent       = 20f;
            var availableWidth = position.width - indent - 8f;
            var cols           = Mathf.Max(1, Mathf.FloorToInt(availableWidth / cellMinWidth));
            var cellWidth      = availableWidth / cols;

            for (var r = 0; r < entry.Rows; r++)
            {
                if (r % cols == 0)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(indent);
                }

                DrawRowOverrideCell(entry, r, cellWidth);

                if (r % cols == cols - 1 || r == entry.Rows - 1)
                    EditorGUILayout.EndHorizontal();
            }
        }

        private void DrawRowOverrideCell(TextureEntry entry, int rowIndex, float cellWidth)
        {
            var row     = entry.RowOverrides[rowIndex];
            var bgRect  = EditorGUILayout.BeginHorizontal(GUILayout.Width(cellWidth), GUILayout.Height(PreviewSize + 4));

            if (row.Enabled)
                EditorGUI.DrawRect(bgRect, RowEnabledColor);

            var previewRect = GUILayoutUtility.GetRect(PreviewSize, PreviewSize, GUIStyle.none,
                GUILayout.Width(PreviewSize), GUILayout.Height(PreviewSize));
            DrawSpritePreview(previewRect, entry, rowIndex);

            GUILayout.Space(4);

            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            row.Enabled = EditorGUILayout.Toggle(row.Enabled, GUILayout.Width(16));
            GUILayout.Label($"행 {rowIndex}", EditorStyles.miniLabel, GUILayout.Width(32));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            row.Label = EditorGUILayout.TextField(row.Label);
            if (GUILayout.Button("▾", EditorStyles.miniButton, GUILayout.Width(22)))
                ShowRowLabelPresetMenu(row, null);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }

        private void ShowRowLabelPresetMenu(RowSetting row, System.Action onChange)
        {
            if (_settings == null) return;
            var menu = new GenericMenu();

            menu.AddItem(new GUIContent("(비움)"), row.Label == "",
                () => { row.Label = ""; onChange?.Invoke(); SaveToSettings(); Repaint(); });

            foreach (var preset in _settings.labelPresets)
            {
                var capture = preset;
                menu.AddItem(new GUIContent(capture), row.Label == capture,
                    () => { row.Label = capture; onChange?.Invoke(); SaveToSettings(); Repaint(); });
            }

            menu.ShowAsContext();
        }

        // ================================================
        // Preset Manager
        // ================================================
        // ================================================
        // Prefix Settings
        // ================================================
        private void DrawPrefixSettings()
        {
            GUILayout.Space(2);
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginHorizontal();

            GUILayout.Label("프리셋", GUILayout.Width(40));
            _prefixPreset = (PrefixPreset)EditorGUILayout.EnumPopup(_prefixPreset, GUILayout.Width(110));

            if (_prefixPreset == PrefixPreset.Custom)
            {
                GUILayout.Space(4);
                _customPrefix = EditorGUILayout.TextField(_customPrefix);
            }

            GUILayout.FlexibleSpace();
            if (GUILayout.Button("전체 적용", EditorStyles.miniButton, GUILayout.Width(60)))
                foreach (var e in _entries) ApplyPrefixPreset(e);

            EditorGUILayout.EndHorizontal();
            if (EditorGUI.EndChangeCheck()) SaveToSettings();
        }

        // ================================================
        // Slice Button
        // ================================================
        private void DrawSliceButton()
        {
            var enabledEntries = _entries.Count(e => e.Enabled);
            var enabledRows    = _entries.Where(e => e.Enabled)
                                         .Sum(e => e.RowOverrides.Count(r => r.Enabled));

            GUI.enabled         = enabledEntries > 0 && enabledRows > 0;
            GUI.backgroundColor = new Color(0.4f, 0.8f, 0.4f);

            if (GUILayout.Button($"Slice  ─  {enabledEntries}개 텍스처  /  총 {enabledRows}개 행", GUILayout.Height(36)))
            {
                AssetDatabase.StartAssetEditing();
                foreach (var entry in _entries.Where(e => e.Enabled))
                    SliceEntry(entry);
                AssetDatabase.StopAssetEditing();
                AssetDatabase.Refresh();
            }

            GUI.backgroundColor = Color.white;
            GUI.enabled         = true;
        }

        // ================================================
        // Slice
        // ================================================
        private void SliceEntry(TextureEntry entry)
        {
            if (AssetImporter.GetAtPath(entry.Path) is not TextureImporter importer) return;

            var validRows = entry.RowOverrides
                .Select((r, i) => (r, i))
                .Where(t => t.r.Enabled)
                .ToList();

            var multiRow    = validRows.Count > 1;
            var spriteRects = new List<SpriteRect>();

            foreach (var (rowSetting, rowIndex) in validRows)
            {
                var y         = (entry.Rows - 1 - rowIndex) * entry.SpriteHeight;
                var rowSuffix = !string.IsNullOrEmpty(rowSetting.Label)
                    ? $"_{rowSetting.Label}"
                    : multiRow ? $"_R{rowIndex}" : "";

                for (var col = 0; col < entry.Cols; col++)
                {
                    spriteRects.Add(new SpriteRect
                    {
                        name      = $"{entry.Prefix}{rowSuffix}_{col}",
                        rect      = new Rect(col * entry.SpriteWidth, y, entry.SpriteWidth, entry.SpriteHeight),
                        alignment = SpriteAlignment.Center,
                        pivot     = new Vector2(0.5f, 0.5f),
                        spriteID  = GUID.Generate(),
                    });
                }
            }

            importer.textureType      = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Multiple;

            var factory = new SpriteDataProviderFactories();
            factory.Init();
            var dataProvider = factory.GetSpriteEditorDataProviderFromObject(importer);
            dataProvider.InitSpriteEditorDataProvider();
            dataProvider.SetSpriteRects(spriteRects.ToArray());
            dataProvider.Apply();

            EditorUtility.SetDirty(importer);
            importer.SaveAndReimport();

            Debug.Log($"<color=green>Slice 완료</color> [{entry.Texture.name}] → {spriteRects.Count}개 스프라이트");
        }

        // ================================================
        // Helpers
        // ================================================
        private static void DrawSpritePreview(Rect rect, TextureEntry entry, int row)
        {
            EditorGUI.DrawRect(rect, new Color(0.12f, 0.12f, 0.12f));
            if (row >= entry.Rows) return;

            var tex = entry.Texture;
            var y   = (entry.Rows - 1 - row) * entry.SpriteHeight;
            GUI.DrawTextureWithTexCoords(rect, tex, new Rect(
                0f,
                (float)y / tex.height,
                (float)entry.SpriteWidth / tex.width,
                (float)entry.SpriteHeight / tex.height
            ));
        }

        private void ApplyPrefixPreset(TextureEntry entry)
        {
            entry.Prefix = _prefixPreset switch
            {
                PrefixPreset.TextureName => entry.Texture.name,
                PrefixPreset.Custom      => _customPrefix,
                _                        => entry.Texture.name,
            };
        }

        private static void DrawSectionHeader(string title)
        {
            GUILayout.Label(title, EditorStyles.boldLabel);
            DrawSeparator();
        }

        private static void DrawSeparator()
        {
            var rect = EditorGUILayout.GetControlRect(false, 1);
            EditorGUI.DrawRect(rect, SeparatorColor);
            GUILayout.Space(2);
        }
    }
}
