using System;
using FMODUnity;
using TBM.MaouSuika.Core.Audio;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    /// <summary>
    /// 싱글 플레이 기준으로 구현됨.
    /// 이후 멀티 플레이 추가 시,
    /// danger level 계산 기능과 snapshot 요청 기능을 분리하고
    /// 상위 시스템이 필요한 계산 모듈을 사용하여 해당 모드에 맞게 초기화하도록 구현필요.
    /// </summary>
    public class GlobalDangerAudioSystem : MonoBehaviour
    {
        [SerializeField] private DangerLevelEventChannelSO dangerLevelEventChannel;

        [SerializeField] private EventReference dangerSnapshot;

        private DangerLevel _lastDangerLevel;

        private void OnEnable()
        {
            dangerLevelEventChannel.EventRaised += HandleDangerLevelChanged;
        }

        public void Initialize()
        {
            AudioManager.Instance.SetSnapshotActive(dangerSnapshot, true);
        }

        private void OnDisable()
        {
            dangerLevelEventChannel.EventRaised -= HandleDangerLevelChanged;
        }

        private void OnDestroy()
        {
            AudioManager.Instance.SetSnapshotActive(dangerSnapshot, false);
        }

        private void HandleDangerLevelChanged(int playerId, DangerLevel dangerLevel)
        {
            if (dangerLevel == _lastDangerLevel) return;

            ApplyDangerLevel(dangerLevel);

            _lastDangerLevel = dangerLevel;
        }

        private void ApplyDangerLevel(DangerLevel level)
        {
            AudioManager.Instance.SetSnapshotParameter(dangerSnapshot, FmodParamNames.DANGER_LEVEL,
                level.ToFmodLabel());
        }
    }
}