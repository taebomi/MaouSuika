using FMODUnity;
using SOSG.Stage.GameOver;
using SOSG.System.Audio;
using UnityEngine;

namespace SOSG.Stage.SplitScreenMode
{
    public class BgmManager : MonoBehaviour
    {
        [SerializeField] private DangerStateBgmController dangerStateBgmController;
        
        [SerializeField] private EventReference bgmRef;

        public void SetUp(PlayerSuikaManager[] curPlayerArr)
        {
            dangerStateBgmController.SetUp(curPlayerArr);
        }

        public void OnGameStarted()
        {
            AudioSystemHelper.PlayBgm(bgmRef);
        }
        
    }
}