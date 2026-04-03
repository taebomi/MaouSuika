using SOSG.Stage.SplitScreenMode;
 using SOSG.Stage.Suika.Shooter;
 using UnityEngine;
using UnityEngine.Serialization;

namespace SOSG.Stage.Suika
{
    public class SuikaSystem : MonoBehaviour
    {
        [SerializeField] private SuikaShooterTouchManager touchManager;
        [SerializeField] private SuikaQueueManager suikaQueueManager;
        [SerializeField] private SuikaPool suikaPool;

        [field:SerializeField] public PlayerSuikaManager[] PlayerArr { get; private set; }
        public PlayerSuikaManager[] CurPlayerArr { get; private set; }

        public void SetUp(int playerNum)
        {
            CurPlayerArr = new PlayerSuikaManager[playerNum];
            for (var i = 0; i < playerNum; i++)
            {
                CurPlayerArr[i] = PlayerArr[i];
                CurPlayerArr[i].SetUp();
            }
            suikaQueueManager.SetUp(CurPlayerArr);

            touchManager.SetUp(playerNum);
        }

        public void TearDown()
        {
            touchManager.TearDown();
        }


        public void Ready()
        {
            foreach (var playerSuika in CurPlayerArr)
            {
                if (playerSuika.gameObject.activeSelf is false)
                {
                    continue;
                }
                playerSuika.OnGameReady();
            }
        }

        public void OnStartGame()
        {
        }
    }
}