using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace TBM.MaouSuika.Dev
{
    public class MonsterAnimCreator : EditorWindow
    {
        // 개별 작업을 관리할 클래스
        [System.Serializable]
        public class WorkItem
        {
            public Texture2D texture;
            public int fps;

            public WorkItem(Texture2D tex, int defaultFps)
            {
                texture = tex;
                fps = defaultFps;
            }
        }

        private string _childPath = "Body/Sprite";
        private int _defaultFrameRate = 10; // 기본값

        // 작업 목록 리스트
        private List<WorkItem> _workItems = new();
        private Vector2 _scrollPos;

        [MenuItem("TBM/Maou Suika/Monster Anim Creator")]
        public static void ShowWindow()
        {
            GetWindow<MonsterAnimCreator>("Monster Anim Creator");
        }

        private void OnEnable()
        {
            // 선택 변경 시 리스트 갱신 이벤트 등록
            OnSelectionChange();
        }

        // 프로젝트 창에서 선택이 바뀔 때마다 호출
        private void OnSelectionChange()
        {
            var selectedTextures = Selection.objects.OfType<Texture2D>().ToArray();

            _workItems.Clear();
            foreach (var tex in selectedTextures)
            {
                _workItems.Add(new WorkItem(tex, _defaultFrameRate));
            }

            Repaint(); // UI 갱신
        }

        private void OnGUI()
        {
            GUILayout.Label("스프라이트 시트 -> 애니메이션 변환기 (Final)", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("규칙:\n1. Die: 전체 통으로 사용\n2. Idle: 마지막 2장(뒷모습) 제외 후 반띵\n3. 나머지: 그냥 반띵 (앞=좌측, 뒤=우측)",
                MessageType.Info);

            GUILayout.Space(10);

            // --- 글로벌 설정 ---
            _childPath = EditorGUILayout.TextField("자식 경로", _childPath);

            GUILayout.BeginHorizontal();
            _defaultFrameRate = EditorGUILayout.IntField("기본 FPS", _defaultFrameRate);
            if (GUILayout.Button("목록 전체 적용", GUILayout.Width(100)))
            {
                foreach (var item in _workItems) item.fps = _defaultFrameRate;
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            GUILayout.Label($"선택된 파일: {_workItems.Count}개", EditorStyles.boldLabel);

            // --- 파일 목록 (스크롤) ---
            _scrollPos = GUILayout.BeginScrollView(_scrollPos, GUILayout.Height(300));
            if (_workItems.Count > 0)
            {
                for (int i = 0; i < _workItems.Count; i++)
                {
                    var item = _workItems[i];
                    GUILayout.BeginHorizontal("box");

                    // 텍스처 이름
                    EditorGUILayout.LabelField(item.texture.name, GUILayout.Width(200));

                    // 개별 FPS 입력
                    GUILayout.Label("FPS:", GUILayout.Width(30));
                    item.fps = EditorGUILayout.IntField(item.fps, GUILayout.Width(50));

                    GUILayout.EndHorizontal();
                }
            }
            else
            {
                GUILayout.Label("프로젝트 창에서 텍스처(시트)를 선택하세요.", EditorStyles.helpBox);
            }

            GUILayout.EndScrollView();

            GUILayout.Space(10);

            // --- 실행 버튼 ---
            GUI.enabled = _workItems.Count > 0;
            if (GUILayout.Button("설정된 대로 변환하기", GUILayout.Height(40)))
            {
                ProcessWorkItems();
            }

            GUI.enabled = true;
        }

        private void ProcessWorkItems()
        {
            foreach (var item in _workItems)
            {
                // 각 아이템의 개별 FPS를 전달
                CreateAnimFromSheet(item.texture, item.fps);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("모든 작업 완료!");
        }

        // fps 파라미터 추가
        private void CreateAnimFromSheet(Texture2D texture, int fps)
        {
            string assetPath = AssetDatabase.GetAssetPath(texture);
            string fileName = texture.name;

            // 1. 스프라이트 로드 및 정렬
            var allSprites = AssetDatabase.LoadAllAssetsAtPath(assetPath)
                .OfType<Sprite>()
                .OrderBy(s => s.name, new NaturalSortComparer())
                .ToArray();

            if (allSprites.Length == 0)
            {
                Debug.LogWarning($"{fileName}: 스프라이트가 없습니다. Slice 상태를 확인하세요.");
                return;
            }

            // 2. 타입 분석
            string[] nameParts = fileName.Split('_');
            string animType = nameParts.Length > 1 ? nameParts[nameParts.Length - 1] : fileName;

            bool isIdle = animType.IndexOf("Idle", System.StringComparison.OrdinalIgnoreCase) >= 0;
            bool isDie = animType.IndexOf("Die", System.StringComparison.OrdinalIgnoreCase) >= 0;

            // 3. 로직 분기
            if (isDie)
            {
                // [Case 1] Die: 통으로 생성
                CreateClip(assetPath, fileName, allSprites, false, fps);
            }
            else if (isIdle)
            {
                // [Case 2] Idle: 이름으로 명확하게 구분
                // 일반 Left, Right 찾기 (Back이 포함되지 않은 것만!)
                var normalLeft = allSprites.Where(s => s.name.Contains("Left") && !s.name.Contains("Back")).ToArray();
                var normalRight = allSprites.Where(s => s.name.Contains("Right") && !s.name.Contains("Back")).ToArray();

                
                if (normalLeft.Length > 0) CreateClip(assetPath, $"{fileName}_Left", normalLeft, true, fps);
                if (normalRight.Length > 0) CreateClip(assetPath, $"{fileName}_Right", normalRight, true, fps);
            }
            else
            {
                // [Case 3] Move, Hit 등: Back 없음, 그냥 Left/Right 구분
                var leftSprites = allSprites.Where(s => s.name.Contains("Left")).ToArray();
                var rightSprites = allSprites.Where(s => s.name.Contains("Right")).ToArray();

                if (leftSprites.Length > 0) CreateClip(assetPath, $"{fileName}_Left", leftSprites, true, fps);
                if (rightSprites.Length > 0) CreateClip(assetPath, $"{fileName}_Right", rightSprites, true, fps);
            }
        }

        private void CreateClip(string texturePath, string clipName, Sprite[] sprites, bool isLoop, int fps)
        {
            // 1. 현재 텍스처가 있는 폴더
            string currentFolder = Path.GetDirectoryName(texturePath);

            // 2. 그 상위 폴더
            string monsterRootFolder = Path.GetDirectoryName(currentFolder);

            // 3. 상위 폴더 아래에 Animations 폴더 경로
            string targetFolder = Path.Combine(monsterRootFolder, "Animations");

            if (!Directory.Exists(targetFolder))
            {
                Directory.CreateDirectory(targetFolder);
            }

            string savePath = Path.Combine(targetFolder, clipName + ".anim");

            AnimationClip clip = new AnimationClip();
            clip.frameRate = fps; // ⭐ 개별 FPS 적용

            if (isLoop)
            {
                AnimationClipSettings settings = AnimationUtility.GetAnimationClipSettings(clip);
                settings.loopTime = true;
                AnimationUtility.SetAnimationClipSettings(clip, settings);
            }

            ObjectReferenceKeyframe[] keyFrames = new ObjectReferenceKeyframe[sprites.Length];
            for (int i = 0; i < sprites.Length; i++)
            {
                keyFrames[i] = new ObjectReferenceKeyframe();
                // 개별 FPS 기준 시간 계산
                keyFrames[i].time = i / (float)fps;
                keyFrames[i].value = sprites[i];
            }

            EditorCurveBinding binding = new EditorCurveBinding();
            binding.type = typeof(SpriteRenderer);
            binding.path = _childPath;
            binding.propertyName = "m_Sprite";

            AnimationUtility.SetObjectReferenceCurve(clip, binding, keyFrames);

            AssetDatabase.CreateAsset(clip, savePath);
            Debug.Log($"생성: {clipName}.anim (FPS: {fps})");
        }

        public class NaturalSortComparer : IComparer<string>
        {
            [System.Runtime.InteropServices.DllImport("shlwapi.dll",
                CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
            private static extern int StrCmpLogicalW(string psz1, string psz2);

            public int Compare(string a, string b) => StrCmpLogicalW(a, b);
        }
    }
}