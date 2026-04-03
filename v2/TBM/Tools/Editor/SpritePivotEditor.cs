using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.U2D.Sprites;
using UnityEngine;

namespace TBM.Tools.Editor
{
    public class SpritePivotEditor : EditorWindow
    {
        private class SpriteItem
        {
            public string Name;
            public Texture2D Texture;
            public Vector2 Pivot;

            // 구분용 플래그
            public bool IsSingleMode;

            // Single/Multiple 공통 처리를 위한 Rect (텍스처 내 좌표)
            public Rect Rect;

            // Multiple 모드일 때만 사용
            public SpriteMetaData MetaData;
        }

        private readonly List<SpriteItem> _allItems = new();

        // 공용 뷰 제어 변수
        private Vector2 _mainScrollPos;
        private Vector2 _commonPreviewScroll = Vector2.zero;
        private float _commonZoom = 3f;
        private float _previewSize = 150f;

        // 클립보드
        private static Vector2? _clipboardPivot = null;

        [MenuItem("TBM/Tools/Sprite Pivot Simple Editor")]
        public static void ShowWindow()
        {
            GetWindow<SpritePivotEditor>("Pivot Editor");
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
            _allItems.Clear();

            var selectedTextures = Selection.GetFiltered<Texture2D>(SelectionMode.Assets);

            var factory = new SpriteDataProviderFactories();
            factory.Init();

            foreach (var texture in selectedTextures)
            {
                string path = AssetDatabase.GetAssetPath(texture);
                TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;

                if (importer == null) continue;

                // 1. Single 모드 처리
                if (importer.spriteImportMode == SpriteImportMode.Single)
                {
                    SpriteItem item = new SpriteItem();
                    item.Name = texture.name;
                    item.Texture = texture;
                    item.IsSingleMode = true;

                    var settings = new TextureImporterSettings();
                    importer.ReadTextureSettings(settings);

                    item.Pivot = settings.spritePivot;
                    // Single은 텍스처 전체를 사용
                    item.Rect = new Rect(0, 0, texture.width, texture.height);

                    _allItems.Add(item);
                }
                // 2. Multiple 모드 처리
                else if (importer.spriteImportMode == SpriteImportMode.Multiple)
                {
                    var dataProvider = factory.GetSpriteEditorDataProviderFromObject(importer);
                    dataProvider.InitSpriteEditorDataProvider();

                    var spriteRects = dataProvider.GetSpriteRects();
                    foreach (var spriteRect in spriteRects)
                    {
                        var item = new SpriteItem();
                        item.Name = spriteRect.name;
                        item.Texture = texture;
                        item.IsSingleMode = false;

                        item.Pivot = spriteRect.pivot;
                        item.Rect = spriteRect.rect;

                        _allItems.Add(item);
                    }
                }
            }
        }

        private void OnGUI()
        {
            GUILayout.Label("통합 스프라이트 피벗 에디터 (Single + Multiple)", EditorStyles.boldLabel);

            DrawTopControlBar();

            if (_allItems.Count == 0)
            {
                GUILayout.Space(20);
                EditorGUILayout.HelpBox("프로젝트 창에서 텍스처(Sprite)를 선택하세요.\n(Single, Multiple 모두 지원)", MessageType.Info);
                return;
            }

            GUILayout.Space(10);

            _mainScrollPos = GUILayout.BeginScrollView(_mainScrollPos);

            // 텍스처 별로 그룹핑
            var textureGroups = _allItems.GroupBy(x => x.Texture);

            foreach (var textureGroup in textureGroups)
            {
                Texture2D tex = textureGroup.Key;
                if (tex == null) continue;

                GUILayout.BeginVertical("helpbox");

                // 헤더 표시 (Single인지 Multiple인지 정보 추가)
                string modeLabel = textureGroup.First().IsSingleMode ? "[Single]" : "[Multiple]";
                GUILayout.Label($"📦 {tex.name} {modeLabel}", EditorStyles.boldLabel);
                GUILayout.Space(5);

                DrawGrid(textureGroup.ToList());

                GUILayout.EndVertical();
                GUILayout.Space(10);
            }

            GUILayout.EndScrollView();
        }

        private void DrawTopControlBar()
        {
            GUILayout.BeginHorizontal("box");

            GUILayout.Label("Size:", GUILayout.Width(35));
            _previewSize = EditorGUILayout.Slider(_previewSize, 100f, 400f, GUILayout.Width(150));

            GUILayout.Space(10);
            GUILayout.Label("Zoom:", GUILayout.Width(40));
            _commonZoom = EditorGUILayout.Slider(_commonZoom, 1f, 20f, GUILayout.Width(150));

            GUILayout.FlexibleSpace();

            GUI.enabled = _clipboardPivot.HasValue;
            if (GUILayout.Button("Paste All", GUILayout.Height(20), GUILayout.Width(100)))
            {
                if (EditorUtility.DisplayDialog("일괄 적용", "리스트에 있는 모든 스프라이트의 피벗을 변경하시겠습니까?", "Yes", "No"))
                {
                    if (_clipboardPivot.HasValue)
                    {
                        foreach (var item in _allItems) item.Pivot = _clipboardPivot.Value;
                    }
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
        }

        private void DrawGrid(List<SpriteItem> items)
        {
            float windowWidth = position.width - 40;
            float itemWidth = _previewSize + 10f;
            float spacing = 10f;

            int columns = Mathf.Max(1, Mathf.FloorToInt(windowWidth / (itemWidth + spacing)));

            for (int i = 0; i < items.Count; i++)
            {
                if (i % columns == 0) GUILayout.BeginHorizontal();

                DrawItem(items[i]);
                GUILayout.Space(spacing);

                if (i % columns == columns - 1 || i == items.Count - 1)
                {
                    GUILayout.EndHorizontal();
                    GUILayout.Space(spacing);
                }
            }
        }

        private void DrawItem(SpriteItem item)
        {
            GUILayout.BeginVertical("box", GUILayout.Width(_previewSize));

            GUILayout.Label(item.Name, EditorStyles.miniLabel);

            GUILayout.BeginHorizontal();
            Vector2 newPivot = EditorGUILayout.Vector2Field("", item.Pivot, GUILayout.Width(60));
            if (newPivot != item.Pivot) item.Pivot = newPivot;

            if (GUILayout.Button("C", EditorStyles.miniButton, GUILayout.Width(20))) _clipboardPivot = item.Pivot;

            GUI.enabled = _clipboardPivot.HasValue;
            if (GUILayout.Button("P", EditorStyles.miniButton, GUILayout.Width(20))) item.Pivot = _clipboardPivot.Value;
            GUI.enabled = true;
            GUILayout.EndHorizontal();

            float viewportHeight = _previewSize;
            Rect viewportRect = GUILayoutUtility.GetRect(_previewSize, viewportHeight);
            EditorGUI.DrawRect(viewportRect, new Color(0.15f, 0.15f, 0.15f, 1f));

            GUI.BeginGroup(viewportRect);
            HandleInput(viewportRect, item);
            DrawSpriteContent(viewportRect, item);
            GUI.EndGroup();

            GUILayout.EndVertical();
        }

        private void HandleInput(Rect viewportRect, SpriteItem item)
        {
            Event e = Event.current;
            Vector2 localMousePos = e.mousePosition;

            // Single이든 Multiple이든 미리 계산해둔 Rect 사용
            float imgWidth = item.Rect.width * _commonZoom;
            float imgHeight = item.Rect.height * _commonZoom;

            Vector2 centerOffset = new Vector2(viewportRect.width * 0.5f, viewportRect.height * 0.5f) -
                                   (new Vector2(imgWidth, imgHeight) * 0.5f);
            Vector2 finalPos = centerOffset + _commonPreviewScroll;
            Rect imageRect = new Rect(finalPos.x, finalPos.y, imgWidth, imgHeight);
            Rect hitRect = new Rect(0, 0, viewportRect.width, viewportRect.height);

            if (hitRect.Contains(localMousePos))
            {
                if (e.type == EventType.ScrollWheel)
                {
                    _commonZoom = Mathf.Clamp(_commonZoom - e.delta.y * 0.5f, 1f, 30f);
                    e.Use();
                }
                else if (e.type == EventType.MouseDrag && (e.button == 2 || (e.alt && e.button == 0)))
                {
                    _commonPreviewScroll += e.delta;
                    e.Use();
                }
                else if ((e.type == EventType.MouseDown || e.type == EventType.MouseDrag) && e.button == 0 && !e.alt)
                {
                    float normalizedX = (localMousePos.x - imageRect.x) / imageRect.width;
                    float normalizedY = 1f - ((localMousePos.y - imageRect.y) / imageRect.height);

                    // Rect 정보 사용
                    float spriteWidth = item.Rect.width;
                    float spriteHeight = item.Rect.height;

                    float pixelX = Mathf.Round(normalizedX * spriteWidth * 2f) / 2f;
                    float pixelY = Mathf.Round(normalizedY * spriteHeight * 2f) / 2f;

                    item.Pivot = new Vector2(
                        Mathf.Clamp01(pixelX / spriteWidth),
                        Mathf.Clamp01(pixelY / spriteHeight)
                    );

                    Repaint();
                }
            }
        }

        private void DrawSpriteContent(Rect viewportRect, SpriteItem item)
        {
            Texture2D tex = item.Texture;

            // UV 계산 (Single은 0~1 전체, Multiple은 조각)
            Rect texCoords;
            if (item.IsSingleMode)
            {
                texCoords = new Rect(0, 0, 1, 1);
            }
            else
            {
                texCoords = new Rect(
                    item.Rect.x / tex.width,
                    item.Rect.y / tex.height,
                    item.Rect.width / tex.width,
                    item.Rect.height / tex.height
                );
            }

            float imgWidth = item.Rect.width * _commonZoom;
            float imgHeight = item.Rect.height * _commonZoom;

            Vector2 centerOffset = new Vector2(viewportRect.width * 0.5f, viewportRect.height * 0.5f) -
                                   (new Vector2(imgWidth, imgHeight) * 0.5f);
            Vector2 finalPos = centerOffset + _commonPreviewScroll;
            Rect drawRect = new Rect(finalPos.x, finalPos.y, imgWidth, imgHeight);

            GUI.DrawTextureWithTexCoords(drawRect, tex, texCoords);
            Handles.DrawSolidRectangleWithOutline(drawRect, Color.clear, new Color(1, 1, 1, 0.3f));

            float pX = drawRect.x + drawRect.width * item.Pivot.x;
            float pY = drawRect.y + drawRect.height * (1f - item.Pivot.y);

            Handles.color = Color.cyan;
            float crossSize = 1000f;
            Handles.DrawLine(new Vector3(pX - crossSize, pY), new Vector3(pX + crossSize, pY));
            Handles.DrawLine(new Vector3(pX, pY - crossSize), new Vector3(pX, pY + crossSize));
            Handles.color = Color.yellow;
            Handles.DrawWireDisc(new Vector3(pX, pY), Vector3.forward, 3f);
        }

        private void ApplyAllChanges()
        {
            // 경로별로 아이템 분류
            var itemsByPath = _allItems.GroupBy(x => AssetDatabase.GetAssetPath(x.Texture));
            int count = 0;

            AssetDatabase.StartAssetEditing();

            var factory = new SpriteDataProviderFactories();
            factory.Init();

            foreach (var group in itemsByPath)
            {
                string path = group.Key;
                TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
                if (importer == null) continue;

                bool isDirty = false;

                // 1. Single Mode 저장 로직
                if (importer.spriteImportMode == SpriteImportMode.Single)
                {
                    // Single은 파일당 아이템이 1개뿐임
                    var item = group.First();

                    var settings = new TextureImporterSettings();
                    importer.ReadTextureSettings(settings);


                    if (importer.spritePivot != item.Pivot)
                    {
                        settings.spriteAlignment = (int)SpriteAlignment.Custom; // 세팅 객체를 통해 설정
                        settings.spritePivot = item.Pivot;

                        importer.SetTextureSettings(settings); // 다시 적용
                        isDirty = true;
                    }
                }
                // 2. Multiple Mode 저장 로직
                else if (importer.spriteImportMode == SpriteImportMode.Multiple)
                {
                    var dataProvider = factory.GetSpriteEditorDataProviderFromObject(importer);
                    dataProvider.InitSpriteEditorDataProvider();

                    var rectProvider = dataProvider.GetDataProvider<ISpriteEditorDataProvider>();
                    var spriteRects = rectProvider.GetSpriteRects();

                    foreach (var item in group)
                    {
                        for (var i = 0; i < spriteRects.Length; i++)
                        {
                            if (spriteRects[i].name != item.Name) continue;

                            var rect = spriteRects[i];
                            rect.alignment = SpriteAlignment.Custom;
                            rect.pivot = item.Pivot;
                            spriteRects[i] = rect;
                            isDirty = true;
                            break;
                        }
                    }
                }

                if (isDirty)
                {
                    EditorUtility.SetDirty(importer);
                    importer.SaveAndReimport();
                    count++;
                }
            }

            AssetDatabase.StopAssetEditing();
            Debug.Log($"✅ 총 {count}개의 텍스처 파일 저장 완료!");
            RefreshSelection();
        }
    }
}