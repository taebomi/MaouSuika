using System;
using Cysharp.Threading.Tasks;
using SOSG.Stage;
using SOSG.Stage.Player;
using SOSG.Stage.Suika;
using SOSG.Stage.Suika.GameOver;
using SOSG.Stage.Suika.Shooter;
using UnityEngine;

namespace SOSG.Stage.SplitScreenMode
{
    public class PlayerSuikaManager : MonoBehaviour
    {
        [field: SerializeField] public PlayerManager PlayerManager { get; private set; }

        public PlayerLoadout Loadout => PlayerManager.Loadout;

        [field: SerializeField] public SuikaQueue Queue { get; private set; }
        [field: SerializeField] public SuikaShooter Shooter { get; private set; }
        [field: SerializeField] public SuikaCreator Creator { get; private set; }
        [field: SerializeField] public SuikaCollection Collection { get; private set; }
        [field: SerializeField] public SuikaMerger Merger { get; private set; }

        [field: SerializeField] public GameOverSystem GameOverSystem { get; private set; }

        public void SetUp()
        {
            Shooter.SetUp();
        }


        public void OnGameReady()
        {
            Shooter.OnGameStarted();
            GameOverSystem.OnStageStarted();
        }

        public async UniTask OnGameOver()
        {
        }
    }
}