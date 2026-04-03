using FMODUnity;
using SOSG.System.Audio;
using UnityEngine;

namespace SOSG.System.Dialogue
{
    public class ConversationSfxController : MonoBehaviour
    {
        [SerializeField] private LineSfxDataSO overlordLineSfxData;
        
        private EventReference _curToneSfxRef;

        public void PlaySfx()
        {
            AudioSystemHelper.PlayOverlordSfx(_curToneSfxRef);
        }

        public void ResetSfx()
        {
            _curToneSfxRef = overlordLineSfxData.normal;
        }

        public void SetTone(string tone)
        {
            _curToneSfxRef = tone switch
            {
                "low" => overlordLineSfxData.low,
                "mid" => overlordLineSfxData.normal,
                "high" => overlordLineSfxData.high,
                _ => _curToneSfxRef
            };
        }
    }
}