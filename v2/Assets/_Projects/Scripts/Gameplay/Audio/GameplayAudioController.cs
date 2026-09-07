using System;
using FMODUnity;
using MaouSuika.Core;
using R3;
using UnityEngine;

namespace MaouSuika.Gameplay.Audio
{
    public class GameplayAudioController : MonoBehaviour
    {
        [SerializeField] private EventReference dangerSnapshot;

        public void Initialize()
        {
            AudioManager.Instance.StartSnapshot(dangerSnapshot);
        }

        private void OnDestroy()
        {
            var audioManager = AudioManager.Instance;
            if (audioManager) audioManager.StopSnapshot(dangerSnapshot);
        }

        public void PlayBgm(EventReference bgm)
        {
            AudioManager.Instance.PlayBgm(bgm);
        }

        public void SetGameOverPhase(GameOverPhase phase)
        {
            var dangerLevel = phase switch
            {
                GameOverPhase.None => 0f,
                GameOverPhase.Warning => 1f,
                GameOverPhase.Countdown => 2f,
                GameOverPhase.GameOver => 0f,
                _ => throw new ArgumentOutOfRangeException(nameof(phase), phase, null),
            };

            AudioManager.Instance.SetSnapshot(dangerSnapshot, FmodParamNames.DANGER_LEVEL, dangerLevel);
        }
    }
}