using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TBM.MaouSuika.Data;
using TBM.MaouSuika.Gameplay.Monster;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TBM.MaouSuika.Dev
{
    public class MonsterAnimDataGeneratorWindow : EditorWindow
    {// 사용자가 드래그해서 넣은 폴더들을 저장할 리스트
        private List<DefaultAsset> _targetFolders = new List<DefaultAsset>();
        private Vector2 _scrollPos;

        [MenuItem("TBM/Maou Suika/Monster Anim Data Generator (List)")]
        public static void ShowWindow()
        {
            var window = GetWindow<MonsterAnimDataGeneratorWindow>("AnimData Gen");
            window.minSize = new Vector2(350, 500);
        }

        private void OnGUI()
        {
            GUILayout.Space(10);
            GUILayout.Label("몬스터 데이터 일괄 생성기", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("아래 박스 영역에 몬스터 폴더(또는 Animations 폴더)를\n드래그해서 놓으세요.", MessageType.Info);
            
            GUILayout.Space(10);

            // 1. 드래그 앤 드롭 영역 (Drop Zone)
            DrawDropArea();

            GUILayout.Space(10);

            // 2. 리스트 컨트롤 (비우기 버튼 등)
            GUILayout.BeginHorizontal();
            GUILayout.Label($"목록 ({_targetFolders.Count})", EditorStyles.boldLabel);
            if (GUILayout.Button("목록 비우기 (Reset)", GUILayout.Width(120)))
            {
                _targetFolders.Clear();
            }
            GUILayout.EndHorizontal();

            // 3. 리스트 출력
            DrawFolderList();

            GUILayout.Space(10);

            // 4. 실행 버튼
            GUI.enabled = _targetFolders.Count > 0;
            if (GUILayout.Button("리스트에 있는 모든 몬스터 데이터 생성", GUILayout.Height(50)))
            {
                ProcessAllFolders();
            }
            GUI.enabled = true;
        }

        private void DrawDropArea()
        {
            Event evt = Event.current;
            Rect dropArea = GUILayoutUtility.GetRect(0.0f, 60.0f, GUILayout.ExpandWidth(true));
            GUI.Box(dropArea, "\n📂 여기로 폴더들을 드래그하세요", EditorStyles.helpBox);

            switch (evt.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!dropArea.Contains(evt.mousePosition))
                        return;

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (evt.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();

                        foreach (Object draggedInter in DragAndDrop.objectReferences)
                        {
                            DefaultAsset folderAsset = draggedInter as DefaultAsset;
                            if (folderAsset != null)
                            {
                                string path = AssetDatabase.GetAssetPath(folderAsset);
                                if (AssetDatabase.IsValidFolder(path))
                                {
                                    // 중복 체크 후 추가
                                    if (!_targetFolders.Contains(folderAsset))
                                    {
                                        _targetFolders.Add(folderAsset);
                                    }
                                }
                            }
                        }
                    }
                    break;
            }
        }

        private void DrawFolderList()
        {
            _scrollPos = GUILayout.BeginScrollView(_scrollPos, "box", GUILayout.Height(300));
            
            if (_targetFolders.Count == 0)
            {
                GUILayout.Label("목록이 비어있습니다.", EditorStyles.centeredGreyMiniLabel);
            }

            for (int i = 0; i < _targetFolders.Count; i++)
            {
                GUILayout.BeginHorizontal();
                
                // 폴더 Object Field (직접 넣을 수도 있게)
                _targetFolders[i] = (DefaultAsset)EditorGUILayout.ObjectField(_targetFolders[i], typeof(DefaultAsset), false);

                // 삭제 버튼
                if (GUILayout.Button("X", GUILayout.Width(25)))
                {
                    _targetFolders.RemoveAt(i);
                    i--; // 리스트 인덱스 조정
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();
        }

        private void ProcessAllFolders()
        {
            int successCount = 0;
            foreach (var folderAsset in _targetFolders)
            {
                if (folderAsset == null) continue;

                string rawPath = AssetDatabase.GetAssetPath(folderAsset);
                
                // 경로 보정 (Slime 폴더를 넣었으면 -> Slime/Animations 로 변환)
                string animFolderPath = GetValidAnimFolderPath(rawPath);

                if (string.IsNullOrEmpty(animFolderPath))
                {
                    Debug.LogWarning($"Animations 폴더를 찾을 수 없음: {rawPath}");
                    continue;
                }

                if (GenerateSO(animFolderPath))
                {
                    successCount++;
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog("완료", $"{successCount}개의 몬스터 데이터(SO)가 처리되었습니다.", "확인");
        }

        // 경로가 Animations 폴더인지, 아니면 그 상위인지 확인해서 실제 Animations 경로 반환
        private string GetValidAnimFolderPath(string path)
        {
            if (path.EndsWith("/Animations", System.StringComparison.OrdinalIgnoreCase))
                return path;
            
            string subPath = Path.Combine(path, "Animations").Replace("\\", "/");
            if (AssetDatabase.IsValidFolder(subPath))
                return subPath;

            return null; // 유효하지 않음
        }
        private bool GenerateSO(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                Debug.LogError("유효하지 않은 폴더 경로입니다.");
                return false;
            }

            // 1. 상위 폴더 경로 및 이름 계산
            string parentFolder = Path.GetDirectoryName(folderPath); // .../Slime
            string monsterName = Path.GetFileName(parentFolder); // Slime
            string soPath = Path.Combine(parentFolder, $"{monsterName}_AnimData.asset");

            // 2. 클립 로드
            string[] guids = AssetDatabase.FindAssets("t:AnimationClip", new[] { folderPath });
            var clips = guids
                .Select(g => AssetDatabase.LoadAssetAtPath<AnimationClip>(AssetDatabase.GUIDToAssetPath(g))).ToArray();

            if (clips.Length == 0)
            {
                Debug.LogWarning("선택된 폴더에 애니메이션 클립이 없습니다.");
                return false;
            }

            // 3. SO 생성 또는 로드
            MonsterAnimDataSO dataSO = AssetDatabase.LoadAssetAtPath<MonsterAnimDataSO>(soPath);
            if (dataSO == null)
            {
                dataSO = ScriptableObject.CreateInstance<MonsterAnimDataSO>();
                AssetDatabase.CreateAsset(dataSO, soPath);
                Debug.Log($"새 SO 생성됨: {soPath}");
            }

            // 4. 데이터 매핑
            SerializedObject so = new SerializedObject(dataSO);
            so.Update();

            // ⭐ Auto-Property 호환 방식 적용
            AssignDirectionalClip(so, "Idle", clips, "Idle");
            AssignDirectionalClip(so, "Move", clips, "Move");
            AssignDirectionalClip(so, "Hit", clips, "Hit");

            // 질문자님 코드에 Jump가 AnimationClip(단일)으로 되어있어 Single로 처리합니다.
            // 만약 Jump도 방향이 필요하면 DirectionalClip으로 바꾸고 AssignDirectionalClip을 쓰세요.
            // (키워드는 일단 "Back"이나 "Jump" 중 하나로 매칭됩니다. 여기선 "Jump"로 설정)
            AssignSingleClip(so, "Jump", clips, "Jump");
            AssignSingleClip(so, "Die", clips, "Die");

            so.ApplyModifiedProperties();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"업데이트 완료: {soPath}");
            Selection.activeObject = dataSO;
            return true;
        }

        // ⭐ 오토 프로퍼티의 Backing Field를 찾는 핵심 함수
        private SerializedProperty FindProperty(SerializedObject so, string propertyName)
        {
            // 1. 일반 필드명 시도 (_idle 등)
            var prop = so.FindProperty(propertyName);
            if (prop != null) return prop;

            // 2. 오토 프로퍼티 Backing Field 이름 시도 (<Idle>k__BackingField)
            var backingFieldName = $"<{propertyName}>k__BackingField";
            prop = so.FindProperty(backingFieldName);

            return prop;
        }

        private void AssignDirectionalClip(SerializedObject so, string propertyName, AnimationClip[] clips,
            string keyword)
        {
            var prop = FindProperty(so, propertyName); // ⭐ 수정된 Find 함수 사용
            if (prop == null)
            {
                Debug.LogError($"프로퍼티 '{propertyName}'를 찾을 수 없습니다.");
                return;
            }

            // 구조체 내부 (DirectionalClip이 일반 필드라면 Right, 오토 프로퍼티라면 <Right>k__BackingField)
            // 보통 구조체 필드는 public AnimationClip Right; 형식을 쓰므로 바로 찾습니다.
            var rightProp = prop.FindPropertyRelative("right");
            var leftProp = prop.FindPropertyRelative("left");

            // 만약 구조체 내부도 오토 프로퍼티라면 아래 주석 해제
            // if (rightProp == null) rightProp = prop.FindPropertyRelative("<Right>k__BackingField");
            // if (leftProp == null) leftProp = prop.FindPropertyRelative("<Left>k__BackingField");

            if (rightProp == null || leftProp == null)
            {
                Debug.LogError($"DirectionalClip 내부 필드(Right/Left)를 찾을 수 없습니다.");
                return;
            }

            foreach (var clip in clips)
            {
                string name = clip.name;
                if (name.IndexOf(keyword, System.StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    if (name.IndexOf("Right", System.StringComparison.OrdinalIgnoreCase) >= 0)
                        rightProp.objectReferenceValue = clip;
                    else if (name.IndexOf("Left", System.StringComparison.OrdinalIgnoreCase) >= 0)
                        leftProp.objectReferenceValue = clip;
                }
            }
        }

        private void AssignSingleClip(SerializedObject so, string propertyName, AnimationClip[] clips, string keyword)
        {
            var prop = FindProperty(so, propertyName); // ⭐ 수정된 Find 함수 사용
            if (prop == null)
            {
                Debug.LogError($"프로퍼티 '{propertyName}'를 찾을 수 없습니다.");
                return;
            }

            foreach (var clip in clips)
            {
                if (clip.name.IndexOf(keyword, System.StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    prop.objectReferenceValue = clip;
                    break;
                }
            }
        }
    }
}