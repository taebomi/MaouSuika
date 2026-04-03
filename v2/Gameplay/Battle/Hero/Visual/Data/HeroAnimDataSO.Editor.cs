#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Battle
{
    public partial class HeroAnimDataSO
    {
        [Button("공격 타이밍 선택")]
        private void OpenAttackTimingSelector()
        {
            if (attack == null)
            {
                Debug.LogWarning("Attack 클립이 없습니다.");
                return;
            }

            AttackTimingSelectorWindow.Open(attack, normalizedTime =>
            {
                AttackTiming = normalizedTime;
                EditorUtility.SetDirty(this);
            });
        }
    }

    public class AttackTimingSelectorWindow : EditorWindow
    {
        private List<(Sprite sprite, float normalizedTime)> _frames;
        private Action<float> _onSelected;

        public static void Open(AnimationClip clip, Action<float> onSelected)
        {
            var window = GetWindow<AttackTimingSelectorWindow>("Attack 타이밍 선택");
            window._onSelected = onSelected;
            window._frames = ExtractFrames(clip);
            window.Show();
        }

        private static List<(Sprite, float)> ExtractFrames(AnimationClip clip)
        {
            var frames = new List<(Sprite, float)>();
            var bindings = AnimationUtility.GetObjectReferenceCurveBindings(clip);

            foreach (var binding in bindings)
            {
                var keyframes = AnimationUtility.GetObjectReferenceCurve(clip, binding);
                foreach (var keyframe in keyframes)
                {
                    if (keyframe.value is Sprite sprite)
                        frames.Add((sprite, keyframe.time / clip.length));
                }
            }

            return frames;
        }

        private void OnGUI()
        {
            if (_frames == null || _frames.Count == 0)
            {
                EditorGUILayout.LabelField("프레임이 없습니다.");
                return;
            }

            const float size = 80f;
            const float padding = 8f;
            int columns = Mathf.Max(1, Mathf.FloorToInt(position.width / (size + padding)));

            for (int i = 0; i < _frames.Count; i += columns)
            {
                GUILayout.BeginHorizontal();
                for (int j = i; j < Mathf.Min(i + columns, _frames.Count); j++)
                {
                    var (sprite, normalizedTime) = _frames[j];
                    DrawFrameButton(sprite, normalizedTime, size);
                }
                GUILayout.EndHorizontal();
            }
        }

        private void DrawFrameButton(Sprite sprite, float normalizedTime, float size)
        {
            GUILayout.BeginVertical(GUILayout.Width(size));

            var rect = GUILayoutUtility.GetRect(size, size, GUILayout.Width(size), GUILayout.Height(size));
            if (GUI.Button(rect, GUIContent.none))
            {
                _onSelected?.Invoke(normalizedTime);
                Close();
            }

            var texCoords = new Rect(
                sprite.rect.x / sprite.texture.width,
                sprite.rect.y / sprite.texture.height,
                sprite.rect.width / sprite.texture.width,
                sprite.rect.height / sprite.texture.height
            );
            GUI.DrawTextureWithTexCoords(rect, sprite.texture, texCoords);

            EditorGUI.LabelField(
                new Rect(rect.x, rect.yMax, rect.width, 16f),
                $"{normalizedTime:F2}",
                new GUIStyle(EditorStyles.centeredGreyMiniLabel)
            );

            GUILayout.Space(18f);
            GUILayout.EndVertical();
        }
    }
}
#endif
