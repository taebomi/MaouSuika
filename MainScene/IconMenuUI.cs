using GooglePlayGames.BasicApi;
using SOSG.GPGS;
using UnityEngine;

namespace SOSG.MainScene
{
    public class IconMenuUI : MonoBehaviour
    {
        [SerializeField] private IntEventSO bgmTimingEventSO;
        
        [SerializeField] private GPGSManagerSO gpgsManagerSO;

        [SerializeField] private Canvas canvas;
        
        [SerializeField] private LocalizationUI localizationUI;

        private void Awake()
        {
            bgmTimingEventSO.OnEventRaised += OnBgmTimingEventRaised;
        }

        private void OnDestroy()
        {
            bgmTimingEventSO.OnEventRaised -= OnBgmTimingEventRaised;
        }

        private void Start()
        {
            if (!MainSceneManager.SkipIntro is false)
            {
                canvas.enabled = true;
            }
            else
            {
                canvas.enabled = false;
            }
        }

        private void OnBgmTimingEventRaised(int timing)
        {
            if (timing == 10)
            {
                canvas.enabled = true;
            }
        }
        public void OnLeaderBoardBtnClicked()
        {
            gpgsManagerSO.ShowLeaderboard(GPGSId.leaderboard, LeaderboardTimeSpan.AllTime, OnLeaderboardShowed);
        }

        private void OnLeaderboardShowed(bool result)
        {
            if (result)
            {
                return;
            }
            
            
        }

        public void OnLanguageBtnClicked()
        {
            localizationUI.Show();
        }
    }
}