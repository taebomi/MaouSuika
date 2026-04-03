using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TBM.Tools.Editor
{
    public class AnimatedTileCreatorWindow : EditorWindow
    {
        // ─── Constants ────────────────────────────────────────────────────────
        private const float MinZoom      = 1f;
        private const float MaxZoom      = 32f;
        private const float RightPanelW  = 280f;
        private const float FrameItemH   = 52f;

        private static readonly Color SeparatorColor  = new(0.25f, 0.25f, 0.25f);
        private static readonly Color GridBgColor     = new(0.15f, 0.15f, 0.15f);
        private static readonly Color HoverColor      = new(1f, 1f, 1f, 0.12f);
        private static readonly Color ActiveBtnColor  = new(0.35f, 0.65f, 1f);
        private static readonly Color SelectionFill   = new(0.3f, 0.7f, 1f, 0.25f);
        private static readonly Color SelectionBorder = new(0.3f, 0.7f, 1f, 0.9f);

        // 프레임별 구분 색상 (순환)
        private static readonly Color[] FrameColors =
        {
            new(0.2f, 0.6f, 1f),
            new(0.2f, 1f, 0.5f),
            new(1f, 0.7f, 0.2f),
            new(1f, 0.3f, 0.5f),
            new(0.8f, 0.4f, 1f),
        };

        private static GUIStyle BadgeStyle => _badgeStyle ??= new GUIStyle(EditorStyles.miniLabel)
            { alignment = TextAnchor.MiddleCenter, normal = { textColor = Color.white } };
        private static GUIStyle _badgeStyle;

        // ─── State ────────────────────────────────────────────────────────────
        private Texture2D    _spriteSheet;
        private List<Sprite> _allSprites = new();

        private float   _zoom           = 1f;
        private bool    _pendingFitZoom;
        private Vector2 _gridScroll;
        private int     _hoveredIdx     = -1;
        private Vector2 _frameListScroll;

        // 줌/패닝 제어
        private float   _panelTop;
        private bool    _isPanning;
        private Vector2 _lastPanMouse;

        // 드래그 영역 선택
        private bool    _isDragSelecting;
        private Vector2 _dragStartScreen;
        private Vector2 _dragCurrentScreen;

        // 프레임 선택 목록: [프레임 인덱스][스프라이트 인덱스]
        // 같은 레이아웃으로 반복 드래그해 프레임을 쌓고, Create 시 위치별로 묶어서 AnimatedTile 생성
        private List<List<Sprite>> _frameSelections = new();

        // AnimatedTile 설정
        private string             _outputName   = "NewAnimatedTile";
        private float              _minSpeed     = 1f;
        private float              _maxSpeed     = 1f;
        private float              _startTime    = 0f;
        private int                _startFrame   = 0;
        private Tile.ColliderType  _colliderType = Tile.ColliderType.None;
        private TileAnimationFlags _animFlags    = TileAnimationFlags.None;

        // ─── Window ───────────────────────────────────────────────────────────

        [MenuItem("TBM/Tools/Animated Tile Creator")]
        public static void ShowWindow() => GetWindow<AnimatedTileCreatorWindow>("Animated Tile Creator");

        private void OnEnable()
        {
            var sel = Selection.activeObject as Texture2D;
            if (sel != null) SetSpriteSheet(sel);
        }

        private void OnSelectionChange()
        {
            var sel = Selection.activeObject as Texture2D;
            if (sel != null && sel != _spriteSheet)
            {
                SetSpriteSheet(sel);
                Repaint();
            }
        }

        // ─── GUI ──────────────────────────────────────────────────────────────

        private void OnGUI()
        {
            if (_pendingFitZoom && _spriteSheet != null)
            {
                var panelW = position.width - RightPanelW - 20f;
                _zoom = Mathf.Clamp(panelW / _spriteSheet.width, MinZoom, MaxZoom);
                _pendingFitZoom = false;
            }

            if (Event.current.type == EventType.Repaint)
                _panelTop = 0f;

            if (_spriteSheet == null || _allSprites.Count == 0)
            {
                EditorGUILayout.HelpBox(
                    _spriteSheet == null
                        ? "프로젝트 뷰에서 Sprite Sheet를 선택하거나, 위 필드에 직접 드래그하세요."
                        : "슬라이스된 스프라이트가 없습니다. Sprite Mode를 Multiple로 설정하세요.",
                    MessageType.Info);
                return;
            }

            HandleSheetInput();

            using (new EditorGUILayout.HorizontalScope())
            {
                DrawSpriteSheetView();
                DrawVSeparator();
                DrawRightPanel();
            }
        }

        // 휠 줌 (마우스 위치 기준 2배 스텝) + 휠클릭 드래그 패닝
        private void HandleSheetInput()
        {
            var e       = Event.current;
            var panelW  = position.width - RightPanelW - 6f;
            var inPanel = e.mousePosition.x < panelW && e.mousePosition.y >= _panelTop;

            // ── 휠 줌 ────────────────────────────────────────────────────────
            if (e.type == EventType.ScrollWheel && inPanel)
            {
                var oldZoom = _zoom;
                var newZoom = Mathf.Clamp(_zoom * (e.delta.y < 0f ? 2f : 0.5f), MinZoom, MaxZoom);

                if (!Mathf.Approximately(newZoom, oldZoom))
                {
                    var mouseInPanel = new Vector2(e.mousePosition.x, e.mousePosition.y - _panelTop);
                    var ratio        = newZoom / oldZoom;
                    _gridScroll.x = (_gridScroll.x + mouseInPanel.x) * ratio - mouseInPanel.x;
                    _gridScroll.y = (_gridScroll.y + mouseInPanel.y) * ratio - mouseInPanel.y;
                    _gridScroll   = Vector2.Max(_gridScroll, Vector2.zero);
                    _zoom         = newZoom;
                }

                e.Use();
                Repaint();
            }

            // ── 휠클릭 드래그 패닝 ──────────────────────────────────────────
            if (e.type == EventType.MouseDown && e.button == 2 && inPanel)
            {
                _isPanning    = true;
                _lastPanMouse = e.mousePosition;
                e.Use();
            }
            else if (e.type == EventType.MouseDrag && e.button == 2 && _isPanning)
            {
                var delta     = e.mousePosition - _lastPanMouse;
                _lastPanMouse = e.mousePosition;
                _gridScroll  -= delta;
                _gridScroll   = Vector2.Max(_gridScroll, Vector2.zero);
                e.Use();
                Repaint();
            }
            else if (e.type == EventType.MouseUp && e.button == 2 && _isPanning)
            {
                _isPanning = false;
                e.Use();
            }
        }

        // ─── Sprite Sheet View ────────────────────────────────────────────────

        private void DrawSpriteSheetView()
        {
            var panelW   = position.width - RightPanelW - 6f;
            var displayW = _spriteSheet.width  * _zoom;
            var displayH = _spriteSheet.height * _zoom;

            using var sv = new EditorGUILayout.ScrollViewScope(
                _gridScroll, GUILayout.Width(panelW), GUILayout.ExpandHeight(true));
            _gridScroll = sv.scrollPosition;

            var texRect = GUILayoutUtility.GetRect(displayW, displayH,
                GUIStyle.none, GUILayout.Width(displayW), GUILayout.Height(displayH));

            // 호버 감지
            var newHovered = -1;
            if (!_isDragSelecting)
            {
                for (var i = 0; i < _allSprites.Count; i++)
                {
                    if (GetSpriteDisplayRect(texRect, _allSprites[i]).Contains(Event.current.mousePosition))
                    {
                        newHovered = i;
                        break;
                    }
                }
            }
            if (_hoveredIdx != newHovered) { _hoveredIdx = newHovered; Repaint(); }

            if (Event.current.type == EventType.Repaint)
            {
                EditorGUI.DrawRect(texRect, GridBgColor);
                GUI.DrawTexture(texRect, _spriteSheet, ScaleMode.StretchToFill);

                // 기존 프레임 선택 오버레이
                for (var f = 0; f < _frameSelections.Count; f++)
                {
                    var fc     = FrameColors[f % FrameColors.Length];
                    var fill   = new Color(fc.r, fc.g, fc.b, 0.18f);
                    var border = new Color(fc.r, fc.g, fc.b, 0.8f);
                    foreach (var sprite in _frameSelections[f])
                    {
                        var sprRect = GetSpriteDisplayRect(texRect, sprite);
                        EditorGUI.DrawRect(sprRect, fill);
                        DrawRectOutline(sprRect, border, 2f);
                        var badge = new Rect(sprRect.xMax - 18, sprRect.y + 2, 16, 16);
                        EditorGUI.DrawRect(badge, border);
                        GUI.Label(badge, (f + 1).ToString(), BadgeStyle);
                    }
                }

                // 호버 하이라이트
                if (_hoveredIdx >= 0)
                    EditorGUI.DrawRect(GetSpriteDisplayRect(texRect, _allSprites[_hoveredIdx]), HoverColor);

                // 드래그 선택 오버레이
                if (_isDragSelecting)
                {
                    var selRect  = GetScreenSelectionRect();
                    var preview  = GetSpritesInRect(texRect, selRect);

                    foreach (var sprite in preview)
                        EditorGUI.DrawRect(GetSpriteDisplayRect(texRect, sprite),
                            new Color(0.3f, 0.7f, 1f, 0.3f));

                    EditorGUI.DrawRect(selRect, SelectionFill);
                    DrawRectOutline(selRect, SelectionBorder, 1.5f);
                }
            }

            var e = Event.current;

            // 우클릭 → 해당 스프라이트가 속한 프레임 전체 제거
            if (e.type == EventType.MouseDown && e.button == 1 && texRect.Contains(e.mousePosition))
            {
                for (var f = _frameSelections.Count - 1; f >= 0; f--)
                {
                    if (_frameSelections[f].Any(s =>
                            GetSpriteDisplayRect(texRect, s).Contains(e.mousePosition)))
                    {
                        _frameSelections.RemoveAt(f);
                        e.Use();
                        Repaint();
                        break;
                    }
                }
            }

            // 드래그 선택 시작
            if (e.type == EventType.MouseDown && e.button == 0 && texRect.Contains(e.mousePosition))
            {
                _isDragSelecting   = true;
                _dragStartScreen   = e.mousePosition;
                _dragCurrentScreen = e.mousePosition;
                e.Use();
            }
            // 드래그 중
            else if (e.type == EventType.MouseDrag && e.button == 0 && _isDragSelecting)
            {
                _dragCurrentScreen = e.mousePosition;
                e.Use();
                Repaint();
            }
            // 드래그 완료 → 프레임 추가
            else if (e.type == EventType.MouseUp && e.button == 0 && _isDragSelecting)
            {
                _isDragSelecting = false;
                var selRect  = GetScreenSelectionRect();
                var selected = GetSpritesInRect(texRect, selRect);
                if (selected.Count > 0)
                {
                    _frameSelections.Add(selected);
                    Repaint();
                }
                e.Use();
            }

            if (_isPanning)
            {
                var panelRect = new Rect(0, _panelTop, panelW, position.height - _panelTop);
                EditorGUIUtility.AddCursorRect(panelRect, MouseCursor.Pan);
            }
        }

        private Rect GetScreenSelectionRect()
        {
            var minX = Mathf.Min(_dragStartScreen.x, _dragCurrentScreen.x);
            var minY = Mathf.Min(_dragStartScreen.y, _dragCurrentScreen.y);
            var maxX = Mathf.Max(_dragStartScreen.x, _dragCurrentScreen.x);
            var maxY = Mathf.Max(_dragStartScreen.y, _dragCurrentScreen.y);
            // 최소 1×1 보장 (클릭 시에도 해당 스프라이트 선택 가능)
            if (maxX - minX < 1f) maxX = minX + 1f;
            if (maxY - minY < 1f) maxY = minY + 1f;
            return new Rect(minX, minY, maxX - minX, maxY - minY);
        }

        // 선택 사각형 안에 중심점이 포함된 스프라이트를 읽기 순서(위→아래, 좌→우)로 반환
        private List<Sprite> GetSpritesInRect(Rect texRect, Rect selRect)
        {
            return _allSprites
                .Where(s => selRect.Overlaps(GetSpriteDisplayRect(texRect, s)))
                .OrderBy(s => -s.textureRect.y)
                .ThenBy(s => s.textureRect.x)
                .ToList();
        }

        private Rect GetSpriteDisplayRect(Rect texDisplayRect, Sprite sprite)
        {
            var tr   = sprite.textureRect;
            var texW = (float)_spriteSheet.width;
            var texH = (float)_spriteSheet.height;
            return new Rect(
                texDisplayRect.x + tr.x / texW * texDisplayRect.width,
                texDisplayRect.y + (1f - (tr.y + tr.height) / texH) * texDisplayRect.height,
                tr.width  / texW * texDisplayRect.width,
                tr.height / texH * texDisplayRect.height);
        }

        // ─── Right Panel ──────────────────────────────────────────────────────

        private void DrawRightPanel()
        {
            using (new EditorGUILayout.VerticalScope(GUILayout.Width(RightPanelW)))
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.Label("Frame Selections", EditorStyles.boldLabel);
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Clear", EditorStyles.miniButton, GUILayout.Width(42)))
                    { _frameSelections.Clear(); Repaint(); }
                }

                GUILayout.Label(GetFrameSummary(), EditorStyles.miniLabel);
                GUILayout.Space(2);

                using (var sv = new EditorGUILayout.ScrollViewScope(_frameListScroll, GUILayout.ExpandHeight(true)))
                {
                    _frameListScroll = sv.scrollPosition;
                    if (_frameSelections.Count == 0)
                        EditorGUILayout.HelpBox("왼쪽 시트에서 드래그해 프레임 영역을 추가하세요.", MessageType.None);
                    else
                        DrawFrameList();
                }

                DrawHSeparator();
                DrawSettings();
                DrawCreateButton();
                GUILayout.Space(6);
            }
        }

        private string GetFrameSummary()
        {
            if (_frameSelections.Count == 0) return "0 프레임 선택됨";
            var counts = _frameSelections.Select(f => f.Count).Distinct().ToList();
            if (counts.Count > 1)
                return $"{_frameSelections.Count} 프레임 — 스프라이트 수 불일치!";
            return $"{_frameSelections.Count} 프레임 × {counts[0]} 스프라이트 → {counts[0]}개 AnimatedTile 생성";
        }

        private void DrawFrameList()
        {
            for (var f = 0; f < _frameSelections.Count; f++)
            {
                var frame = _frameSelections[f];
                var fc    = FrameColors[f % FrameColors.Length];

                using (new EditorGUILayout.HorizontalScope(GUILayout.Height(FrameItemH)))
                {
                    // 색상 바
                    var colorBarRect = GUILayoutUtility.GetRect(4, FrameItemH, GUILayout.Width(4));
                    if (Event.current.type == EventType.Repaint)
                        EditorGUI.DrawRect(colorBarRect, fc);

                    GUILayout.Space(4);

                    using (new EditorGUILayout.VerticalScope(GUILayout.ExpandWidth(true)))
                    {
                        GUILayout.Space(2);
                        GUILayout.Label($"Frame {f + 1}  ({frame.Count} sprites)", EditorStyles.miniLabel);

                        // 썸네일 스트립
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            var maxThumb  = Mathf.Min(frame.Count, 8);
                            var thumbSize = FrameItemH - 22f;
                            for (var i = 0; i < maxThumb; i++)
                            {
                                var tr = GUILayoutUtility.GetRect(thumbSize, thumbSize,
                                    GUILayout.Width(thumbSize), GUILayout.Height(thumbSize));
                                if (Event.current.type == EventType.Repaint)
                                    DrawSpritePreview(tr, frame[i]);
                            }
                            if (frame.Count > 8)
                                GUILayout.Label($"+{frame.Count - 8}", EditorStyles.miniLabel);
                        }
                        GUILayout.Space(2);
                    }

                    // 삭제 버튼
                    if (GUILayout.Button("×", EditorStyles.miniButton,
                            GUILayout.Width(18), GUILayout.Height(18)))
                    {
                        _frameSelections.RemoveAt(f);
                        Repaint();
                        break;
                    }
                    GUILayout.Space(2);
                }

                EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1),
                    new Color(0.3f, 0.3f, 0.3f));
            }
        }

        private void DrawSettings()
        {
            const float labelW = 60f;
            const float fieldW = 48f;

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label("Min Speed", EditorStyles.miniLabel, GUILayout.Width(labelW));
                _minSpeed = EditorGUILayout.FloatField(_minSpeed, GUILayout.Width(fieldW));
                GUILayout.Space(4);
                GUILayout.Label("Max Speed", EditorStyles.miniLabel, GUILayout.Width(labelW));
                _maxSpeed = EditorGUILayout.FloatField(_maxSpeed, GUILayout.Width(fieldW));
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label("Start Time", EditorStyles.miniLabel, GUILayout.Width(labelW));
                _startTime = EditorGUILayout.FloatField(_startTime, GUILayout.Width(fieldW));
                GUILayout.Space(4);
                GUILayout.Label("Start Frame", EditorStyles.miniLabel, GUILayout.Width(labelW));
                _startFrame = EditorGUILayout.IntField(_startFrame, GUILayout.Width(fieldW));
            }

            GUILayout.Space(6);

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label("Collider", EditorStyles.miniLabel, GUILayout.Width(labelW));
                if (ToggleButton("None",   _colliderType == Tile.ColliderType.None))
                    _colliderType = Tile.ColliderType.None;
                if (ToggleButton("Sprite", _colliderType == Tile.ColliderType.Sprite))
                    _colliderType = Tile.ColliderType.Sprite;
                if (ToggleButton("Grid",   _colliderType == Tile.ColliderType.Grid))
                    _colliderType = Tile.ColliderType.Grid;
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label("Anim Flags", EditorStyles.miniLabel, GUILayout.Width(labelW));
                if (ToggleButton("None",     _animFlags == TileAnimationFlags.None))
                    _animFlags = TileAnimationFlags.None;
                if (ToggleButton("LoopOnce", (_animFlags & TileAnimationFlags.LoopOnce) != 0))
                    _animFlags ^= TileAnimationFlags.LoopOnce;
                if (ToggleButton("Pause",    (_animFlags & TileAnimationFlags.PauseAnimation) != 0))
                    _animFlags ^= TileAnimationFlags.PauseAnimation;
            }
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Space(labelW + 4);
                if (ToggleButton("Physics",  (_animFlags & TileAnimationFlags.UpdatePhysics) != 0))
                    _animFlags ^= TileAnimationFlags.UpdatePhysics;
                if (ToggleButton("Unscaled", (_animFlags & TileAnimationFlags.UnscaledTime) != 0))
                    _animFlags ^= TileAnimationFlags.UnscaledTime;
                if (ToggleButton("Sync",     (_animFlags & TileAnimationFlags.SyncAnimation) != 0))
                    _animFlags ^= TileAnimationFlags.SyncAnimation;
            }

            GUILayout.Space(6);

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label("이름", EditorStyles.miniLabel, GUILayout.Width(labelW));
                _outputName = EditorGUILayout.TextField(_outputName);
            }

            GUILayout.Space(4);
        }

        private void DrawCreateButton()
        {
            var isValid = ValidateFrameSelections(out var error);

            if (!isValid && _frameSelections.Count > 0)
                EditorGUILayout.HelpBox(error, MessageType.Warning);

            GUI.enabled         = isValid;
            GUI.backgroundColor = new Color(0.7f, 1f, 0.7f);

            var spriteCount = isValid ? _frameSelections[0].Count : 0;
            var btnLabel    = spriteCount > 1
                ? $"Create {spriteCount} AnimatedTiles ({_outputName}_1 ~ _{spriteCount})"
                : "Create AnimatedTile";

            if (GUILayout.Button(btnLabel, GUILayout.Height(36)))
                CreateAllAnimatedTiles();

            GUI.backgroundColor = Color.white;
            GUI.enabled         = true;
        }

        // ─── Validation & Create ──────────────────────────────────────────────

        private bool ValidateFrameSelections(out string error)
        {
            if (_frameSelections.Count == 0)
            {
                error = "프레임 선택이 없습니다.";
                return false;
            }
            var counts = _frameSelections.Select(f => f.Count).Distinct().ToList();
            if (counts.Count > 1)
            {
                error = "프레임마다 스프라이트 수가 달라 생성할 수 없습니다. " +
                        $"({string.Join(", ", _frameSelections.Select(f => f.Count))})";
                return false;
            }
            error = null;
            return true;
        }

        private void CreateAllAnimatedTiles()
        {
            if (!ValidateFrameSelections(out _)) return;

            var baseFolder = GetBaseFolder();
            if (baseFolder == null) return;

            EnsureFolder(baseFolder);

            var spriteCount = _frameSelections[0].Count;
            for (var s = 0; s < spriteCount; s++)
            {
                var sequence  = _frameSelections.Select(f => f[s]).ToArray();
                var tileName  = spriteCount > 1 ? $"{_outputName}_{s + 1}" : _outputName;
                var assetPath = $"{baseFolder}/{tileName}.asset";
                CreateAnimatedTile(assetPath, sequence);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"<color=green>AnimatedTile {spriteCount}개 생성 완료</color>  폴더: {baseFolder}");
        }

        private void CreateAnimatedTile(string assetPath, Sprite[] sequence)
        {
            var existing = AssetDatabase.LoadAssetAtPath<AnimatedTile>(assetPath);
            var target   = existing ?? ScriptableObject.CreateInstance<AnimatedTile>();

            target.m_AnimatedSprites    = sequence;
            target.m_MinSpeed           = _minSpeed;
            target.m_MaxSpeed           = _maxSpeed;
            target.m_AnimationStartTime = _startTime;
            target.m_TileColliderType   = _colliderType;

            var so             = new SerializedObject(target);
            var startFrameProp = so.FindProperty("m_AnimationStartFrame");
            if (startFrameProp != null) startFrameProp.intValue = _startFrame;
            var flagsProp = so.FindProperty("m_TileAnimationFlags");
            if (flagsProp != null) flagsProp.intValue = (int)_animFlags;
            so.ApplyModifiedPropertiesWithoutUndo();

            if (existing == null)
                AssetDatabase.CreateAsset(target, assetPath);
            else
                EditorUtility.SetDirty(existing);

            EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<AnimatedTile>(assetPath));
        }

        // ─── Sprite Sheet ─────────────────────────────────────────────────────

        private void SetSpriteSheet(Texture2D tex)
        {
            _spriteSheet = tex;
            _allSprites.Clear();
            _frameSelections.Clear();

            if (tex == null) return;

            var path = AssetDatabase.GetAssetPath(tex);
            _allSprites = AssetDatabase.LoadAllAssetsAtPath(path)
                .OfType<Sprite>()
                .OrderBy(s => -s.textureRect.y)
                .ThenBy(s => s.textureRect.x)
                .ToList();

            _outputName     = tex.name + "_AnimTile";
            _pendingFitZoom = true;
        }

        private string GetBaseFolder()
        {
            if (_spriteSheet == null) return null;
            var sheetPath = AssetDatabase.GetAssetPath(_spriteSheet);
            return Path.GetDirectoryName(sheetPath)?.Replace('\\', '/');
        }

        // ─── Helpers ──────────────────────────────────────────────────────────

        private static bool ToggleButton(string label, bool isOn)
        {
            GUI.backgroundColor = isOn ? ActiveBtnColor : Color.white;
            var clicked = GUILayout.Button(label, EditorStyles.miniButton);
            GUI.backgroundColor = Color.white;
            return clicked;
        }

        private static void DrawSpritePreview(Rect rect, Sprite sprite)
        {
            if (sprite == null) return;
            var tex = sprite.texture;
            var tr  = sprite.textureRect;
            GUI.DrawTextureWithTexCoords(rect, tex, new Rect(
                tr.x / tex.width,     tr.y / tex.height,
                tr.width / tex.width, tr.height / tex.height));
        }

        private static void DrawRectOutline(Rect r, Color c, float t)
        {
            EditorGUI.DrawRect(new Rect(r.x,        r.y,        r.width, t),        c);
            EditorGUI.DrawRect(new Rect(r.x,        r.yMax - t, r.width, t),        c);
            EditorGUI.DrawRect(new Rect(r.x,        r.y,        t, r.height),       c);
            EditorGUI.DrawRect(new Rect(r.xMax - t, r.y,        t, r.height),       c);
        }

        private static void DrawHSeparator()
        {
            GUILayout.Space(4);
            EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), SeparatorColor);
            GUILayout.Space(4);
        }

        private static void DrawVSeparator()
        {
            var rect = GUILayoutUtility.GetRect(1, 1, GUIStyle.none,
                GUILayout.Width(1), GUILayout.ExpandHeight(true));
            EditorGUI.DrawRect(rect, SeparatorColor);
        }

        private static void EnsureFolder(string folderPath)
        {
            if (string.IsNullOrEmpty(folderPath) || AssetDatabase.IsValidFolder(folderPath)) return;
            var parent = Path.GetDirectoryName(folderPath)?.Replace('\\', '/');
            var name   = Path.GetFileName(folderPath);
            if (!string.IsNullOrEmpty(parent) && !string.IsNullOrEmpty(name))
                AssetDatabase.CreateFolder(parent, name);
        }
    }
}
