using System;
using FMODUnity;
using MaouSuika.Core;
using Sirenix.OdinInspector;
using TBM.Pool;
using UnityEngine;

namespace MaouSuika.Gameplay
{
    public class SoulOrbPopEffectPlayer : MonoBehaviour
    {
        /*
         * TODO: 등급별 커스터마이즈 지원 시
         * 배열 인스펙터 직렬화 대신 Setup(EffectSet) 주입으로 교체 및 분리
         */

        private const string COMBO_PARAMETER = "Combo";

        [SerializeField] private SoulOrbPopCameraShakeSettingsSO cameraShakeSettings;

        [SerializeField] private EventReference popSfx;
        [SerializeField] private EventReference finaleSfx;

        [SerializeField] private GradeEffect[] entries;
        [SerializeField] private SoulOrbPopEffectFinale finalePrefab;

        private SelfReleasingPool<SoulOrbPopEffectGradeBase>[] _gradePools;
        private SelfReleasingPool<SoulOrbPopEffectFinale> _finalePool;

        private Action<CameraShakeRequest> _requestCameraShake;
        private Action<HapticRequest> _requestHaptics;

        public void Initialize(Action<CameraShakeRequest> requestCameraShake, Action<HapticRequest> requestHaptics)
        {
            _requestCameraShake = requestCameraShake;
            _requestHaptics = requestHaptics;

            _gradePools = new SelfReleasingPool<SoulOrbPopEffectGradeBase>[SoulOrbTiers.GRADE_COUNT];
            for (var i = 0; i < entries.Length; i++)
            {
                var gradeIdx = (int)entries[i].grade;
                _gradePools[gradeIdx] =
                    new SelfReleasingPool<SoulOrbPopEffectGradeBase>(entries[i].effectPrefab, transform,
                        5);
            }

            _finalePool =
                new SelfReleasingPool<SoulOrbPopEffectFinale>(finalePrefab, transform, 1);
        }

        public void PlayFinale(Vector2 position)
        {
            var effect = _finalePool.Get();
            effect.Play(position);
            AudioManager.Instance.PlaySfxOneShot(finaleSfx);
            _requestHaptics(default);

            var cameraShakeRequest = cameraShakeSettings.RequestFor(SoulOrbTiers.MAX_TIER, 0);
            _requestCameraShake(cameraShakeRequest);
        }

        public void PlayMerge(in SoulOrbPopEffectInfo popEffectInfo)
        {
            Play(popEffectInfo);
            _requestHaptics(default);


            var cameraShakeRequest = cameraShakeSettings.RequestFor(popEffectInfo.Tier, popEffectInfo.Combo);
            _requestCameraShake(cameraShakeRequest);
        }

        public void Play(in SoulOrbPopEffectInfo popEffectInfo)
        {
            var pool = _gradePools[(int)popEffectInfo.Grade];
            var effect = pool.Get();
            effect.Play(popEffectInfo.Position, popEffectInfo.Color, popEffectInfo.Size);
            AudioManager.Instance.PlaySfxOneShot(popSfx, COMBO_PARAMETER, popEffectInfo.Combo);
        }

        [Serializable]
        private struct GradeEffect
        {
            public Grade grade;
            public SoulOrbPopEffectGradeBase effectPrefab;
        }

#if UNITY_EDITOR

        private void OnValidate()
        {
            if (entries == null) return;
            var seen = new bool[SoulOrbTiers.GRADE_COUNT];
            foreach (var entry in entries)
            {
                var idx = (int)entry.grade;
                if (seen[idx]) Debug.LogError($"{entry.grade} 등급이 중복이야!", this);
                seen[idx] = true;
            }

            for (var i = 0; i < seen.Length; i++)
            {
                if (!seen[i]) Debug.LogError($"{(Grade)i} 등급이 누락됐어!", this);
            }
        }

        [Button]
        [EnableIf("@UnityEngine.Application.isPlaying")]
        private void TestCameraShakeAndHaptics(
            [PropertyRange(0, SoulOrbTiers.MAX_TIER)] int tier,
            [PropertyRange(0, ComboGrades.MAX_MIN_COMBO)] int combo)
        {
            var request = cameraShakeSettings.RequestFor(tier, combo);

            _requestCameraShake(request);
            _requestHaptics(default);
        }
#endif
    }
}