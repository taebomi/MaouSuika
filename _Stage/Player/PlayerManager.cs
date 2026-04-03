using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using SOSG.Monster;
using SOSG.Stage.SplitScreenMode;
using SOSG.System.Loadout;
using SOSG.System.PlayData;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace SOSG.Stage.Player
{
    public class PlayerManager : MonoBehaviour, IPlayerGameOverHandler
    {
        [field: SerializeField] public PlayerSuikaManager SuikaManager { get; private set; }
        [field: SerializeField] public PlayerLoadout Loadout { get; private set; }

        public int PlayerIdx { get; private set; }

        public async UniTask SetUpAsync(PlayerSetUpData setUpData)
        {
            PlayerIdx = setUpData.playerIdx;
            await Loadout.SetUpAsync(setUpData.loadoutString);
        }

        public async UniTask OnGameOverAsync()
        {
            await SuikaManager.OnGameOver();
        }
    }
}