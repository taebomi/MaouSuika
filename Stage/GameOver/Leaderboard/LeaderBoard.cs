using GooglePlayGames.BasicApi;
using SOSG.GPGS;
using SOSG.System;
using TaeBoMi;
using UnityEngine;

namespace SOSG.Stage.GameOver
{
    public class LeaderBoard : MonoBehaviour
    {
        [SerializeField] private StageManagerSO stageManagerSO;

        [SerializeField] private GPGSManagerSO gpgsManagerSO;

        [SerializeField] private ObscuredIntVarSO curScore;

        private void OnEnable()
        {
            stageManagerSO.ActionOnStageEnded += OnStageEnded;
        }

        private void OnDisable()
        {
            stageManagerSO.ActionOnStageEnded -= OnStageEnded;
        }


        public void OnLeaderBoardBtnClicked()
        {
            gpgsManagerSO.ShowLeaderboard(GPGSId.leaderboard, LeaderboardTimeSpan.Daily,OnLeaderBoardShowed);
        }

        private void OnLeaderBoardShowed(bool result)
        {
            if (result)
            {
                return;
            }
            
            
        }

        private void OnStageEnded()
        {
            TBMUtility.Log($"# Leaderboard - Report Score");
            int score = curScore;
            gpgsManagerSO.ReportScore(score, GPGSId.leaderboard);
        }
    }
}