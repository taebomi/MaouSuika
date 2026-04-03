using System;
using Cysharp.Threading.Tasks;
using SOSG.Stage;
using SOSG.Stage.GameOver;
using SOSG.Stage.Suika;
using SOSG.System.Loadout;
using SOSG.System.PlayData;
using SOSG.System.Scene;
using UnityEngine;

namespace SOSG.Stage.SplitScreenMode
{
    public class SplitScreenModeManager : MonoBehaviour
    {
        [SerializeField] private SplitScreenModePlayDataVarSO playData;

        [SerializeField] private BgmManager bgmManager;
        [SerializeField] private DangerStateBgmController dangerStateBgmController;
        
        [SerializeField] private SuikaSystem suikaSystem;
        
        private void Awake()
        {
            SceneSetUpHelper.AddTask(SetUpAsync);
            SceneSetUpHelper.Completed += OnSceneSetUp;
        }

        private async UniTask SetUpAsync()
        {
            
        }

        private void OnDestroy()
        {
            suikaSystem.TearDown();
            dangerStateBgmController.TearDown();
        }

        private void OnSceneSetUp()
        {
            suikaSystem.SetUp(playData.playerNum);
            bgmManager.SetUp( suikaSystem.CurPlayerArr);
            suikaSystem.Ready();
            suikaSystem.OnStartGame();
            bgmManager.OnGameStarted();
        }
        
    }
}