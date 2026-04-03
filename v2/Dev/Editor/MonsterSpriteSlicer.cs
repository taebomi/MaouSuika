using UnityEngine;
using UnityEditor;
using UnityEditor.U2D.Sprites; // ⭐ 필수 네임스페이스
using System.Collections.Generic;
using System.Linq;

namespace TBM.MaouSuika.Dev
{
    public class MonsterSpriteSlicer : EditorWindow
    {
        // 피벗 기본값
        private SpriteAlignment _pivot = SpriteAlignment.BottomCenter;

        [MenuItem("TBM/Maou Suika/Monster Sprite Slicer")]
        public static void ShowWindow()
        {
            GetWindow<MonsterSpriteSlicer>("Monster Slicer");
        }

        private void OnGUI()
        {
            GUILayout.Label("몬스터 스프라이트 오토 슬라이서 (ISpriteDataProvider)", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "규칙 (정사각형 셀 전제):\n" +
                "1. Die: 1행 (전체)\n" +
                "2. Move, Hit: 4행 중 1행(Right), 2행(Left)만\n" +
                "3. Idle: Move 규칙 + 3,4행의 첫 프레임(점프 예비)",
                MessageType.Info);

            GUILayout.Space(10);
            _pivot = (SpriteAlignment)EditorGUILayout.EnumPopup("Pivot 설정", _pivot);

            GUILayout.Space(20);

            if (GUILayout.Button("선택한 텍스처 슬라이스 실행", GUILayout.Height(40)))
            {
                SliceSelectedTextures();
            }
        }

        private void SliceSelectedTextures()
        {
            var selectedTextures = Selection.GetFiltered<Texture2D>(SelectionMode.Assets);

            if (selectedTextures.Length == 0)
            {
                Debug.LogWarning("선택된 텍스처가 없습니다.");
                return;
            }

            int count = 0;
            AssetDatabase.StartAssetEditing(); // 대량 작업 최적화

            // DataProvider 팩토리 초기화
            var factory = new SpriteDataProviderFactories();
            factory.Init();

            foreach (var texture in selectedTextures)
            {
                string path = AssetDatabase.GetAssetPath(texture);
                TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;

                if (importer == null) continue;

                // 1. 임포터 기본 설정 (Multiple 모드)
                SetupImporter(importer);

                // 2. 새로운 슬라이싱 데이터 계산
                List<SpriteRect> newSpriteRects = CalculateSlicing(texture, importer);

                // 3. 데이터 주입 및 적용 (ISpriteEditorDataProvider 사용)
                ApplySlicing(importer, factory, newSpriteRects);

                // 4. 저장
                importer.SaveAndReimport();
                count++;
            }

            AssetDatabase.StopAssetEditing();
            Debug.Log($"✅ 슬라이스 완료: {count}개 파일 처리됨.");
        }

        private void SetupImporter(TextureImporter importer)
        {
            if (importer.textureType != TextureImporterType.Sprite ||
                importer.spriteImportMode != SpriteImportMode.Multiple)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Multiple;
                // 설정을 바꿨다면 1차 적용이 필요할 수 있음 (Provider가 초기화되기 위해)
                importer.SaveAndReimport();
            }
        }

        // ⭐ 순수 로직: 텍스처 정보를 받아 SpriteRect 리스트를 반환
        private List<SpriteRect> CalculateSlicing(Texture2D texture, TextureImporter importer)
        {
            string fileName = texture.name;
            string[] nameParts = fileName.Split('_');
            string animType = nameParts.Length > 1 ? nameParts[nameParts.Length - 1] : fileName;

            // TextureImporter를 통해 실제 텍스처 크기를 가져오는 것이 안전함 (isReadable false 대응)
            importer.GetSourceTextureWidthAndHeight(out int width, out int height);

            List<SpriteRect> spriteRects = new List<SpriteRect>();
            Vector2 pivotVec = GetPivotValue(_pivot);

            // 타입별 분기 처리
            if (animType.Contains("Die"))
            {
                // [Die] 1행 구성 -> 높이가 곧 셀 크기
                int cellSize = height;
                int colCount = width / cellSize;

                // Row 0 (Top) -> 실제 Y = 0 (1행만 있으므로)
                for (int x = 0; x < colCount; x++)
                {
                    AddSpriteRect(spriteRects, fileName, "", x, 0, x, cellSize, height, pivotVec);
                }
            }
            else // Idle, Move, Hit (4행 구성)
            {
                int cellSize = height / 4;
                int colCount = width / cellSize;

                // Row 0 (Top, Right)
                for (int x = 0; x < colCount; x++)
                {
                    AddSpriteRect(spriteRects, fileName, "Right", x, 0, x, cellSize, height, pivotVec);
                }

                // Row 1 (2nd from Top, Left)
                for (int x = 0; x < colCount; x++)
                {
                    AddSpriteRect(spriteRects, fileName, "Left", x, 1, x, cellSize, height, pivotVec);
                }

                // [Idle] 특수 규칙
                if (animType.Contains("Idle"))
                {
                    // Row 2 (3rd from Top, Back Right) -> col 0 only
                    AddSpriteRect(spriteRects, fileName, "BackRight", 0, 2, 0, cellSize, height, pivotVec);

                    // Row 3 (Bottom, Back Left) -> col 0 only
                    AddSpriteRect(spriteRects, fileName, "BackLeft", 0, 3, 0, cellSize, height, pivotVec);
                }
            }

            return spriteRects;
        }

        private void ApplySlicing(TextureImporter importer, SpriteDataProviderFactories factory,
            List<SpriteRect> newRects)
        {
            var dataProvider = factory.GetSpriteEditorDataProviderFromObject(importer);
            dataProvider.InitSpriteEditorDataProvider();
            dataProvider.SetSpriteRects(newRects.ToArray());
            dataProvider.Apply();
        }

        private void AddSpriteRect(List<SpriteRect> rects, string baseName, string suffix, int colIndex,
            int rowIndex, int nameIndex, int size, int texHeight, Vector2 pivot)
        {
            string spriteName = string.IsNullOrEmpty(suffix)
                ? $"{baseName}_{nameIndex}"
                : $"{baseName}_{suffix}_{nameIndex}";

            // Unity Texture 좌표계: 좌하단(0,0) 기준
            // rowIndex 0 (Top) -> y = texHeight - size
            float x = colIndex * size;
            float y = texHeight - (rowIndex + 1) * size;

            SpriteRect newRect = new SpriteRect
            {
                name = spriteName,
                spriteID = GUID.Generate(), // ⭐ SpriteRect는 고유 ID 필수
                rect = new Rect(x, y, size, size),
                alignment = _pivot, // Enum 할당
                pivot = pivot // Vector2 할당 (Custom Alignment 대응)
            };

            rects.Add(newRect);
        }

        private Vector2 GetPivotValue(SpriteAlignment alignment)
        {
            switch (alignment)
            {
                case SpriteAlignment.Center: return new Vector2(0.5f, 0.5f);
                case SpriteAlignment.BottomCenter: return new Vector2(0.5f, 0f);
                case SpriteAlignment.TopCenter: return new Vector2(0.5f, 1f);
                case SpriteAlignment.LeftCenter: return new Vector2(0f, 0.5f);
                case SpriteAlignment.RightCenter: return new Vector2(1f, 0.5f);
                case SpriteAlignment.BottomLeft: return new Vector2(0f, 0f);
                case SpriteAlignment.BottomRight: return new Vector2(1f, 0f);
                case SpriteAlignment.TopLeft: return new Vector2(0f, 1f);
                case SpriteAlignment.TopRight: return new Vector2(1f, 1f);
                default: return new Vector2(0.5f, 0.5f);
            }
        }
    }
}