using Sirenix.OdinInspector;
using TBM.MaouSuika.Core.Save;
using TBM.MaouSuika.Core.Settings;
using UnityEngine;

namespace TBM.MaouSuika.Core.Audio
{
    public class VolumeController : MonoBehaviour
    {
        private VcaChannel _master;
        private VcaChannel _bgm;
        private VcaChannel _sfx;

        public void Initialize(VolumeSettings settings)
        {
            _master = new VcaChannel(FmodPaths.VCA_MASTER, settings.master);
            _bgm = new VcaChannel(FmodPaths.VCA_BGM, settings.bgm);
            _sfx = new VcaChannel(FmodPaths.VCA_SFX, settings.sfx);
        }

        public void SetMasterVolume(float volume)
        {
            _master.SetVolume(volume);
        }

        public void SetBgmVolume(float volume)
        {
            _bgm.SetVolume(volume);
        }

        public void SetSfxVolume(float volume)
        {
            _sfx.SetVolume(volume);
        }

        public void SetMasterMuted(bool isMuted)
        {
            _master.SetMuted(isMuted);
        }

        public void SetBgmMuted(bool isMuted)
        {
            _bgm.SetMuted(isMuted);
        }

        public void SetSfxMuted(bool isMuted)
        {
            _sfx.SetMuted(isMuted);
        }

        public void ApplySettings(VolumeSettings settings)
        {
            _master.ApplySettings(settings.master);
            _bgm.ApplySettings(settings.bgm);
            _sfx.ApplySettings(settings.sfx);
        }

        public VolumeSettings GetSettings()
        {
            return new VolumeSettings(_master.GetSettings(), _bgm.GetSettings(), _sfx.GetSettings());
        }

#if UNITY_EDITOR

// [Master]
        [ShowInInspector]
        [PropertyRange(0f, 1f), LabelText("Volume")] // 슬라이더로 표시
        public float Debug_MasterVolume
        {
            get => _master?.Volume ?? 0f; // 인스펙터가 그려질 때 실제 값 가져옴
            set => SetMasterVolume(value);                // 인스펙터에서 값을 바꾸면 적용 함수 호출
        }

        [ShowInInspector]
        [LabelText("Muted")]
        public bool Debug_MasterMuted
        {
            get => _master is { IsMuted: true };
            set => SetMasterMuted(value);
        }

        // [BGM]
        [ShowInInspector]
        [PropertyRange(0f, 1f), LabelText("Volume")]
        public float Debug_BgmVolume
        {
            get => _bgm?.Volume ?? 0f;
            set => SetBgmVolume(value);
        }

        [ ShowInInspector]
        [LabelText("Muted")]
        public bool Debug_BgmMuted
        {
            get => _bgm is { IsMuted: true };
            set => SetBgmMuted(value);
        }

        [ShowInInspector]
        [PropertyRange(0f, 1f), LabelText("Volume")]
        public float Debug_SfxVolume
        {
            get => _sfx?.Volume ?? 0f;
            set => SetSfxVolume(value);
        }

        [ShowInInspector]
        [LabelText("Muted")]
        public bool Debug_SfxMuted
        {
            get => _sfx is { IsMuted: true };
            set => SetSfxMuted(value);
        }


#endif
    }
}