using UnityEngine;
using UnityEditor;
using UnityEditor.U2D.Sprites; // ⭐ 필수 네임스페이스
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace TBM.MaouSuika.Dev
{
    public class SpritePivotGroupEditor : EditorWindow
    {
        private class PivotGroup
        {
            public string Name;
            public Texture2D Texture;
            public SpriteMetaData FirstFrame;
            public List<SpriteMetaData> AllFrames = new();
            public Vector2 Pivot;
        }

        private List<PivotGroup> _groups = new();

        // Importer 캐싱 대신 Path를 키로 사용 (DataProvider 패턴에 적합)
        private readonly HashSet<string> _selectedAssetPaths = new();

        // UI 상태 변수
        private Vector2 _mainScrollPos;
        private float _previewSize = 250f;

        // 뷰포트 제어용
        private Vector2 _commonScroll = Vector2.zero;
        private float _commonZoom = 8f;

        // 클립보드
        private static Vector2? _clipboardPivot = null;
        private const float VIEWPORT_HEIGHT_RATIO = 1.0f;

        [MenuItem("TBM/Tools/Sprite Pivot Group Editor")]
        public static void ShowWindow()
        {
            GetWindow<SpritePivotGroupEditor>("Pivot Group Editor");
        }

        private void OnSelectionChange()
        {
            RefreshSelection();
            Repaint();
        }

        private void OnEnable()
        {
            RefreshSelection();
        }

        private void RefreshSelection()
        {
            _groups.Clear();
            _selectedAssetPaths.Clear();

            var selectedTextures = Selection.GetFiltered<Texture2D>(SelectionMode.Assets);
            var factory = new SpriteDataProviderFactories();
            factory.Init();

            foreach (var texture in selectedTextures)
            {
                string path = AssetDatabase.GetAssetPath(texture);
                var importer = AssetImporter.GetAtPath(path) as TextureImporter;

                if (importer == null || importer.spriteImportMode != SpriteImportMode.Multiple)
                    continue;

                _selectedAssetPaths.Add(path);

                // ⭐ [Refactor] DataProvider를 사용하여 SpriteRect 데이터 추출
                var dataProvider = factory.GetSpriteEditorDataProviderFromObject(importer);
                dataProvider.InitSpriteEditorDataProvider();

                var spriteRects = dataProvider.GetSpriteRects();

                if (spriteRects == null || spriteRects.Length == 0) continue;

                // 호환성을 위해 SpriteRect -> SpriteMetaData 변환 (UI 로직 유지)
                var sheetData = spriteRects.Select(r => new SpriteMetaData
                {
                    name = r.name,
                    rect = r.rect,
                    alignment = (int)r.alignment,
                    pivot = r.pivot,
                    border = r.border
                }).ToArray();

                var groupedData = GroupSprites(texture, sheetData);
                _groups.AddRange(groupedData);
            }
        }

        private List<PivotGroup> GroupSprites(Texture2D texture, SpriteMetaData[] sheetData)
        {
            Dictionary<string, List<SpriteMetaData>> rawGroups = new Dictionary<string, List<SpriteMetaData>>();
            // 정규식: 이름_숫자 패턴 추출
            Regex reg = new Regex(@"(.*)_(\d+)$");

            foreach (var data in sheetData)
            {
                var match = reg.Match(data.name);
                string key = match.Success ? match.Groups[1].Value : data.name;

                if (!rawGroups.ContainsKey(key))
                    rawGroups[key] = new List<SpriteMetaData>();

                rawGroups[key].Add(data);
            }

            List<PivotGroup> result = new List<PivotGroup>();

            foreach (var kvp in rawGroups)
            {
                var frames = kvp.Value;
                frames.Sort((a, b) => NaturalCompare(a.name, b.name));

                PivotGroup group = new PivotGroup();
                group.Name = kvp.Key.Replace(texture.name + "_", "");
                if (string.IsNullOrEmpty(group.Name)) group.Name = "Default";

                group.Texture = texture;
                group.AllFrames = frames;
                group.FirstFrame = frames[0];
                group.Pivot = frames[0].pivot;

                result.Add(group);
            }

            return result;
        }

        private void OnGUI()
        {
            GUILayout.Label("스프라이트 피벗 에디터 (Production Ready)", EditorStyles.boldLabel);

            DrawTopControlBar();

            if (_groups.Count == 0)
            {
                GUILayout.Space(20);
                GUILayout.Label("선택된 스프라이트 시트가 없습니다. (Multiple Mode Only)", EditorStyles.centeredGreyMiniLabel);
                return;
            }

            GUILayout.Space(10);

            _mainScrollPos = GUILayout.BeginScrollView(_mainScrollPos);

            var textureGroups = _groups.GroupBy(g => g.Texture);

            foreach (var textureGroup in textureGroups)
            {
                Texture2D tex = textureGroup.Key;
                if (tex == null) continue;

                GUILayout.BeginVertical("helpbox");
                GUILayout.Label($"📦 {tex.name}", EditorStyles.boldLabel);
                GUILayout.Space(5);

                float windowWidth = position.width - 30;
                float itemWidth = _previewSize + 20f;
                float spacing = 10f;

                int columns = Mathf.Max(1, Mathf.FloorToInt((windowWidth) / (itemWidth + spacing)));
                var groupList = textureGroup.ToList();

                for (int i = 0; i < groupList.Count; i++)
                {
                    if (i % columns == 0) GUILayout.BeginHorizontal();

                    DrawGroupEditor(groupList[i]);
                    GUILayout.Space(spacing);

                    if (i % columns == columns - 1 || i == groupList.Count - 1)
                    {
                        GUILayout.EndHorizontal();
                        GUILayout.Space(spacing);
                    }
                }

                GUILayout.EndVertical();
                GUILayout.Space(15);
            }

            GUILayout.EndScrollView();
            GUILayout.Space(30);
        }

        private void DrawTopControlBar()
        {
            GUILayout.BeginHorizontal("box");

            GUILayout.Label("Size:", GUILayout.Width(35));
            _previewSize = EditorGUILayout.Slider(_previewSize, 150f, 600f, GUILayout.Width(200));

            GUILayout.FlexibleSpace();

            GUI.enabled = _clipboardPivot.HasValue;
            if (GUILayout.Button(
                    $"Paste All ({(_clipboardPivot.HasValue ? _clipboardPivot.Value.ToString() : "None")})",
                    GUILayout.Height(20), GUILayout.Width(200)))
            {
                if (EditorUtility.DisplayDialog("일괄 적용", "현재 보여지는 모든 그룹의 피벗을 클립보드 값으로 덮어쓰시겠습니까?", "Yes", "No"))
                {
                    foreach (var g in _groups) g.Pivot = _clipboardPivot.Value;
                }
            }

            GUI.enabled = true;

            GUI.backgroundColor = new Color(0.7f, 1f, 0.7f);
            if (GUILayout.Button("Save All", GUILayout.Height(20), GUILayout.Width(100)))
            {
                ApplyAllChanges();
            }

            GUI.backgroundColor = Color.white;

            GUILayout.EndHorizontal();

            EditorGUILayout.HelpBox(
                "• 휠: 줌/이동 (Alt+클릭 이동)  • 좌클릭: 피벗 설정 (0.5픽셀 스냅)\n" +
                "• 변경사항은 'Save All'을 눌러야 반영됩니다.", MessageType.Info);
        }

        private void DrawGroupEditor(PivotGroup group)
        {
            GUILayout.BeginVertical("box", GUILayout.Width(_previewSize));

            GUILayout.Label($"📂 {group.Name}", EditorStyles.boldLabel);

            GUILayout.BeginHorizontal();
            Vector2 newPivot = EditorGUILayout.Vector2Field("", group.Pivot, GUILayout.Width(80));
            if (newPivot != group.Pivot) group.Pivot = newPivot;

            if (GUILayout.Button("B", EditorStyles.miniButtonLeft, GUILayout.Width(25)))
                group.Pivot = new Vector2(0.5f, 0f);
            if (GUILayout.Button("C", EditorStyles.miniButtonMid, GUILayout.Width(25)))
                group.Pivot = new Vector2(0.5f, 0.5f);

            if (GUILayout.Button("Copy", EditorStyles.miniButtonMid, GUILayout.Width(60)))
                _clipboardPivot = group.Pivot;

            GUI.enabled = _clipboardPivot.HasValue;
            if (GUILayout.Button("Paste", EditorStyles.miniButtonRight, GUILayout.Width(60)))
                group.Pivot = _clipboardPivot.Value;
            GUI.enabled = true;

            GUILayout.EndHorizontal();
            GUILayout.Space(5);

            float viewportHeight = _previewSize * VIEWPORT_HEIGHT_RATIO;
            Rect viewportRect = GUILayoutUtility.GetRect(_previewSize, viewportHeight);

            EditorGUI.DrawRect(viewportRect, new Color(0.15f, 0.15f, 0.15f, 1f));

            GUI.BeginGroup(viewportRect);
            HandleInput(viewportRect, group);
            DrawSpriteContent(viewportRect, group);
            GUI.EndGroup();

            GUI.Label(
                new Rect(viewportRect.x + viewportRect.width - 60, viewportRect.y + viewportRect.height - 20, 60, 20),
                $"x{_commonZoom:F1}", EditorStyles.miniLabel);

            GUILayout.EndVertical();
        }

        private void HandleInput(Rect viewportRect, PivotGroup group)
        {
            var e = Event.current;
            var localMousePos = e.mousePosition;

            float imgWidth = group.FirstFrame.rect.width * _commonZoom;
            float imgHeight = group.FirstFrame.rect.height * _commonZoom;

            var centerOffset = new Vector2(viewportRect.width * 0.5f, viewportRect.height * 0.5f) -
                               (new Vector2(imgWidth, imgHeight) * 0.5f);
            var finalPos = centerOffset + _commonScroll;
            var imageRect = new Rect(finalPos.x, finalPos.y, imgWidth, imgHeight);
            var hitRect = new Rect(0, 0, viewportRect.width, viewportRect.height);

            if (hitRect.Contains(localMousePos))
            {
                if (e.type == EventType.ScrollWheel)
                {
                    var multiplier = e.delta.y < 0 ? 2f : 0.5f;
                    _commonZoom = Mathf.Clamp(_commonZoom * multiplier, 4f, 64f);
                    e.Use();
                }
                else if (e.type == EventType.MouseDrag && (e.button == 2 || (e.alt && e.button == 0)))
                {
                    _commonScroll += e.delta;
                    e.Use();
                }
                else if ((e.type == EventType.MouseDown || e.type == EventType.MouseDrag) && e.button == 0 && !e.alt)
                {
                    float normalizedX = (localMousePos.x - imageRect.x) / imageRect.width;
                    float normalizedY = 1f - ((localMousePos.y - imageRect.y) / imageRect.height);

                    float spriteWidth = group.FirstFrame.rect.width;
                    float spriteHeight = group.FirstFrame.rect.height;

                    float pixelX = normalizedX * spriteWidth;
                    float pixelY = normalizedY * spriteHeight;

                    // 0.5 Pixel Snap
                    pixelX = Mathf.Round(pixelX * 2f) / 2f;
                    pixelY = Mathf.Round(pixelY * 2f) / 2f;

                    group.Pivot = new Vector2(Mathf.Clamp01(pixelX / spriteWidth),
                        Mathf.Clamp01(pixelY / spriteHeight));
                    Repaint();
                }
            }
        }

        private void DrawSpriteContent(Rect viewportRect, PivotGroup group)
        {
            SpriteMetaData sprite = group.FirstFrame;
            Texture2D tex = group.Texture;

            Rect texCoords = new Rect(
                sprite.rect.x / tex.width,
                sprite.rect.y / tex.height,
                sprite.rect.width / tex.width,
                sprite.rect.height / tex.height
            );

            float imgWidth = group.FirstFrame.rect.width * _commonZoom;
            float imgHeight = group.FirstFrame.rect.height * _commonZoom;

            Vector2 centerOffset = new Vector2(viewportRect.width * 0.5f, viewportRect.height * 0.5f) -
                                   (new Vector2(imgWidth, imgHeight) * 0.5f);
            Vector2 finalPos = centerOffset + _commonScroll;
            Rect drawRect = new Rect(finalPos.x, finalPos.y, imgWidth, imgHeight);

            GUI.DrawTextureWithTexCoords(drawRect, tex, texCoords);
            Handles.DrawSolidRectangleWithOutline(drawRect, Color.clear, new Color(1, 1, 1, 0.3f));

            float pX = drawRect.x + drawRect.width * group.Pivot.x;
            float pY = drawRect.y + drawRect.height * (1f - group.Pivot.y);

            Handles.color = Color.cyan;
            float crossSize = 1000f;
            Handles.DrawLine(new Vector3(pX - crossSize, pY), new Vector3(pX + crossSize, pY));
            Handles.DrawLine(new Vector3(pX, pY - crossSize), new Vector3(pX, pY + crossSize));

            Handles.color = Color.yellow;
            Handles.DrawWireDisc(new Vector3(pX, pY), Vector3.forward, 3f);

            float pixelX = group.Pivot.x * sprite.rect.width;
            float pixelY = group.Pivot.y * sprite.rect.height;

            GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
            style.normal.textColor = Color.yellow;
            GUI.Box(new Rect(pX + 5, pY - 20, 80, 20), GUIContent.none);
            GUI.Label(new Rect(pX + 10, pY - 20, 80, 20), $"{pixelX:0.0},{pixelY:0.0}", style);
        }

        // ⭐ 핵심 변경: DataProvider API를 통한 저장 로직
        private void ApplyAllChanges()
        {
            AssetDatabase.StartAssetEditing();
            int count = 0;

            // 팩토리 초기화
            var factory = new SpriteDataProviderFactories();
            factory.Init();

            // 텍스처 별로 그룹화하여 처리
            var textureGroups = _groups.GroupBy(g => g.Texture);

            foreach (var texGroup in textureGroups)
            {
                if (texGroup.Key == null) continue;

                string path = AssetDatabase.GetAssetPath(texGroup.Key);
                TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
                if (importer == null) continue;

                // Provider 생성 및 초기화
                var dataProvider = factory.GetSpriteEditorDataProviderFromObject(importer);
                dataProvider.InitSpriteEditorDataProvider();

                // 원본 SpriteRect 리스트 가져오기
                var spriteRects = dataProvider.GetSpriteRects().ToList();
                bool isDirty = false;

                // UI에서 편집된 내용(PivotGroup)을 실제 SpriteRect에 반영
                foreach (var group in texGroup)
                {
                    foreach (var frame in group.AllFrames)
                    {
                        // 이름으로 매칭
                        int index = spriteRects.FindIndex(r => r.name == frame.name);
                        if (index != -1)
                        {
                            var targetRect = spriteRects[index];

                            // 변경 여부 체크 (최적화)
                            if (targetRect.pivot != group.Pivot || targetRect.alignment != SpriteAlignment.Custom)
                            {
                                targetRect.alignment = SpriteAlignment.Custom;
                                targetRect.pivot = group.Pivot;
                                spriteRects[index] = targetRect;
                                isDirty = true;
                            }
                        }
                    }
                }

                if (isDirty)
                {
                    // ⭐ 변경된 데이터를 Provider에 주입
                    dataProvider.SetSpriteRects(spriteRects.ToArray());

                    // Provider 변경사항 적용
                    dataProvider.Apply();

                    // Importer 저장 및 재임포트
                    importer.SaveAndReimport();
                    count++;
                }
            }

            AssetDatabase.StopAssetEditing();

            Debug.Log($"✅ 총 {count}개의 텍스처 파일 업데이트 및 Reimport 완료!");
            RefreshSelection();
        }

        private int NaturalCompare(string a, string b)
        {
            return EditorUtility.NaturalCompare(a, b);
        }
    }
}