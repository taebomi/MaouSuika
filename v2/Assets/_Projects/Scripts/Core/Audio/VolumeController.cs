namespace MaouSuika.Core
{
    public class VolumeController
    {
        private readonly VcaChannel _master;
        private readonly VcaChannel _bgm;
        private readonly VcaChannel _sfx;

        public VolumeController()
        {
            _master = new VcaChannel(FmodPaths.VCA_MASTER);
            _bgm = new VcaChannel(FmodPaths.VCA_BGM);
            _sfx = new VcaChannel(FmodPaths.VCA_SFX);

            ApplySettings(VolumeSettings.CreateDefault());
        }

        public void SetMasterVolume(float volume)
        {
            _master.SetVolume(volume);
        }

        public void SetMasterMuted(bool muted)
        {
            _master.SetMuted(muted);
        }

        public void SetBgmVolume(float volume)
        {
            _bgm.SetVolume(volume);
        }

        public void SetBgmMuted(bool muted)
        {
            _bgm.SetMuted(muted);
        }

        public void SetSfxVolume(float volume)
        {
            _sfx.SetVolume(volume);
        }

        public void SetSfxMuted(bool muted)
        {
            _sfx.SetMuted(muted);
        }

        public void ApplySettings(VolumeSettings settings)
        {
            _master.ApplySettings(settings.master);
            _bgm.ApplySettings(settings.bgm);
            _sfx.ApplySettings(settings.sfx);
        }

        public VolumeSettings CaptureSettings()
        {
            return new VolumeSettings
            {
                master = _master.CaptureSettings(),
                bgm = _bgm.CaptureSettings(),
                sfx = _sfx.CaptureSettings(),
            };
        }
    }
}
