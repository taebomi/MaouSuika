using System;
using System.Collections.Generic;

namespace TBM.MaouSuika.Core.Settings
{
    [Serializable, ES3Serializable]
    public class InputSettings
    {
        // player index에 따른 설정
        // 이후 프로파일 기능 추가 시 수정 필요
        public List<InputProfile> profiles;

        public InputSettings()
        {
            profiles = new List<InputProfile> { new() };
        }

        public InputSettings(InputSettings settings)
        {
            profiles = new List<InputProfile>(settings.profiles.Count);

            foreach (var profile in settings.profiles)
            {
                profiles.Add(new InputProfile(profile));
            }
        }
    }
}