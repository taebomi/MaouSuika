using FMODUnity;
using SOSG.System.Audio;
using UnityEngine;

namespace SOSG.UI
{
    public class UIAudioController : MonoBehaviour
    {
        [SerializeField] private EventReference defaultSfxEventRef;

        public void PlaySfx()
        {
            AudioSystemHelper.PlaySfx(defaultSfxEventRef);
        }
    }
}